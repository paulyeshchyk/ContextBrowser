using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Runtime.Serialization;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Hosting;
using SemanticKit.Model.Options;

namespace RoslynKit.Assembly;
public class AssemblyPathFilter
{
    private readonly SemanticFilters _trustedFilters;
    private readonly SemanticFilters _runtimeFilters;
    private readonly SemanticFilters _domainFilters;

    public AssemblyPathFilter(SemanticFilters trustedFilters, SemanticFilters runtimeFilters, SemanticFilters domainFilters)
    {
        _trustedFilters = trustedFilters;
        _runtimeFilters = runtimeFilters;
        _domainFilters = domainFilters;
    }
    private static IEnumerable<string> ApplyFilters(IEnumerable<string> paths, SemanticFilters filters, string logPrefix, Action<AppLevel, LogLevel, string>? onWriteLog)
    {
        var includedPatterns = filters.Included.Split(';', StringSplitOptions.RemoveEmptyEntries);
        var excludedPatterns = filters.Excluded.Split(';', StringSplitOptions.RemoveEmptyEntries);

        var includedPaths = includedPatterns.Length > 0
            ? paths.Where(path => includedPatterns.Any(pattern => path.Contains(pattern, StringComparison.OrdinalIgnoreCase))).ToArray()
            : paths.ToArray();

        var filteredPaths = excludedPatterns.Length > 0
            ? includedPaths.Where(path => !excludedPatterns.Any(pattern => path.Contains(pattern, StringComparison.OrdinalIgnoreCase))).ToArray()
            : includedPaths.ToArray();

        var excludedLogValue = includedPaths.Except(filteredPaths).Any()
            ? string.Join($"\nExcluded {logPrefix}: ", includedPaths.Except(filteredPaths)) : "none";
        onWriteLog?.Invoke(AppLevel.R_Dll, LogLevel.Trace, $"Excluded {logPrefix}: {excludedLogValue}");

        var includedLogValue = filteredPaths.Any()
            ? string.Join($"\nIncluded {logPrefix}: ", filteredPaths) : "none";
        onWriteLog?.Invoke(AppLevel.R_Dll, LogLevel.Trace, $"Included {logPrefix}: {includedLogValue}");

        return filteredPaths;
    }

    public IEnumerable<string> FilterTrustedPaths(IEnumerable<string> paths, Action<AppLevel, LogLevel, string>? onWriteLog)
    {
        return ApplyFilters(paths, _trustedFilters, "trusted platform assembly", onWriteLog);
    }

    public IEnumerable<string> FilterRuntimePaths(IEnumerable<string> paths, Action<AppLevel, LogLevel, string>? onWriteLog)
    {
        return ApplyFilters(paths, _runtimeFilters, "runtime directory assembly", onWriteLog);
    }

    public IEnumerable<string> FilterDomainPaths(IEnumerable<string> paths, Action<AppLevel, LogLevel, string>? onWriteLog)
    {
        return ApplyFilters(paths, _domainFilters, "current domain assembly", onWriteLog);
    }
}
    // context: roslyn, build
    public static class RoslynAssemblyFetcher
{

    // context: roslyn, build
    public static IEnumerable<MetadataReference> Fetch(AssemblyPathFilter assemblyPathsFilter, OnWriteLog? onWriteLog)
    {
        var trustedPlatformPaths = FetchTrustedPlatformPaths(onWriteLog);
        var filteredTrustedPaths = assemblyPathsFilter.FilterTrustedPaths(trustedPlatformPaths, null);

        var currentDomainPaths = FetchCurrentDomainPaths(onWriteLog);
        var filteredDomainPaths = assemblyPathsFilter.FilterDomainPaths(currentDomainPaths, null);

        var runtimeDirectoryPaths = FetchRuntimeDirectoryAssemblyPaths(onWriteLog);
        var filteredRuntimePaths = assemblyPathsFilter.FilterRuntimePaths(runtimeDirectoryPaths, null);

        var allAssemblyPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            typeof(object).Assembly.Location
        };

        allAssemblyPaths.UnionWith(filteredTrustedPaths);
        allAssemblyPaths.UnionWith(filteredDomainPaths);
        allAssemblyPaths.UnionWith(filteredRuntimePaths);

        return allAssemblyPaths.Select(path => MetadataReference.CreateFromFile(path));
    }

    // context: roslyn, build
    private static string[] FetchTrustedPlatformPaths(OnWriteLog? onWriteLog)
    {
        var trustedAssembliesPaths = (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string ?? string.Empty)
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);

        if (trustedAssembliesPaths.Length == 0)
        {
            onWriteLog?.Invoke(AppLevel.R_Dll, LogLevel.Warn, "Не удалось найти доверенные сборки платформы.");
            return Array.Empty<string>();
        }

        return trustedAssembliesPaths;
    }

    // context: roslyn, build
    private static string[] FetchCurrentDomainPaths(OnWriteLog? onWriteLog)
    {
        var domainAssemblies = AssemblyLoadContext.Default.Assemblies.ToArray();
        var assemblyLocations = new List<string>();

        foreach (var assembly in domainAssemblies)
        {
            try
            {
                if (assembly.IsDynamic)
                {
                    onWriteLog?.Invoke(AppLevel.R_Dll, LogLevel.Trace, $"[SKIP] Dynamic assembly {assembly.FullName}");
                }
                else
                {
                    assemblyLocations.Add(assembly.Location);
                }
            }
            catch (Exception ex)
            {
                onWriteLog?.Invoke(AppLevel.R_Dll, LogLevel.Exception, $"Ошибка при загрузке сборки {assembly.FullName}: {ex.Message}");
            }
        }
        return assemblyLocations.ToArray();
    }

    // context: roslyn, build
    private static string[] FetchRuntimeDirectoryAssemblyPaths(OnWriteLog? onWriteLog)
    {
        var runtimeDirectory = Path.GetDirectoryName(typeof(object).Assembly.Location);
        if (string.IsNullOrWhiteSpace(runtimeDirectory))
        {
            onWriteLog?.Invoke(AppLevel.R_Dll, LogLevel.Warn, "[MISS]: Не найден путь assembly для typeof(object)");
            return Array.Empty<string>();
        }

        var files = Directory.GetFiles(runtimeDirectory, "*.dll", SearchOption.TopDirectoryOnly).ToList();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            files.AddRange(Directory.GetFiles(runtimeDirectory, "*.exe", SearchOption.TopDirectoryOnly));
        }

        return files.ToArray();
    }
}
