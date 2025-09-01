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

// context: roslyn, build
public static class RoslynAssemblyFetcher
{

    // context: roslyn, build
    public static IEnumerable<MetadataReference> Fetch(SemanticFilters semanticFilters, OnWriteLog? onWriteLog)
    {
        var runtimeDirectoryPaths = RoslynAssemblyFetcher.FetchRuntimeDirectoryAssemblyPaths(semanticFilters, onWriteLog);
        var currentDomainPaths = RoslynAssemblyFetcher.FetchCurrentDomainPaths(semanticFilters, onWriteLog);
        //var trustedPlatformPaths = RoslynAssemblyFetcher.FetchTrustedPlatformPaths(semanticFilters, onWriteLog);

        var uniqAssemlyPaths = new HashSet<string>();

        uniqAssemlyPaths.Add(typeof(object).Assembly.Location);

        uniqAssemlyPaths.UnionWith(runtimeDirectoryPaths);
        uniqAssemlyPaths.UnionWith(currentDomainPaths);
        //uniqAssemlyPaths.UnionWith(trustedPlatformPaths);


        return uniqAssemlyPaths.Select(path => MetadataReference.CreateFromFile(path));
    }


    // context: roslyn, build
    internal static string[] FetchTrustedPlatformPaths(SemanticFilters semanticFilters, OnWriteLog? onWriteLog)
    {
        var trustedAssembliesPaths = (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string ?? string.Empty).Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);
        if (trustedAssembliesPaths == null || trustedAssembliesPaths.Length == 0)
        {
            onWriteLog?.Invoke(AppLevel.R_Dll, LogLevel.Warn, "Не удалось найти доверенные сборки платформы.");
            return Array.Empty<string>();
        }

        var excluded = PathFilter.FilterPaths(trustedAssembliesPaths, semanticFilters.ExcludedAssemblyNamesPatterns, (thePath) => (thePath));
        var filtered = PathFilter.FilteroutPaths(trustedAssembliesPaths, semanticFilters.ExcludedAssemblyNamesPatterns, (thePath) => (thePath));

        var excludedLogValue = excluded.Any() ? string.Join("\nExcluded trusted assembly: ", excluded) : "none";
        onWriteLog?.Invoke(AppLevel.R_Dll, LogLevel.Trace, $"Excluded trusted platform assembly: {excludedLogValue}");

        var includedLogValue = filtered.Any() ? string.Join("\nIncluded trusted platform assembly: ", filtered) : "none";
        onWriteLog?.Invoke(AppLevel.R_Dll, LogLevel.Trace, $"Included trusted platform assembly: {includedLogValue}");
        return filtered;
    }

    // context: roslyn, build
    internal static string[] FetchCurrentDomainPaths(SemanticFilters semanticFilters, OnWriteLog? onWriteLog)
    {
        var domainAssemblies = AssemblyLoadContext.Default.Assemblies.ToArray();

        var excluded = PathFilter.FilterPaths(domainAssemblies, semanticFilters.ExcludedAssemblyNamesPatterns, (theAssembly) => (theAssembly.Location));
        var filtered = PathFilter.FilteroutPaths(domainAssemblies, semanticFilters.ExcludedAssemblyNamesPatterns, (theAssembly) => (theAssembly.Location));
        var result = new List<string>();
        foreach (var path in filtered)
        {
            try
            {
                if (!path.IsDynamic)
                {
                    result.Add(path.Location);
                }
                else
                {
                    onWriteLog?.Invoke(AppLevel.R_Dll, LogLevel.Trace, $"[SKIP] Dynamic assembly {path}");
                }
            }
            catch (Exception ex)
            {
                onWriteLog?.Invoke(AppLevel.R_Dll, LogLevel.Exception, $"Ошибка при загрузке сборки {path}: {ex.Message}");
            }
        }

        var excludedLogValue = excluded.Any() ? string.Join("\nExcluded current domain assembly: ", excluded.Select(a => a.Location)) : "none";
        onWriteLog?.Invoke(AppLevel.R_Dll, LogLevel.Trace, $"Excluded current domain assembly: {excludedLogValue}");

        var includedLogValue = result.Any() ? string.Join("\nIncluded current domain assembly: ", result.Select(a => a)) : "none";
        onWriteLog?.Invoke(AppLevel.R_Dll, LogLevel.Trace, $"Included current domain assembly: {includedLogValue}");
        return result.ToArray();
    }

    // context: roslyn, build
    internal static string[] FetchRuntimeDirectoryAssemblyPaths(SemanticFilters semanticFilters, OnWriteLog? onWriteLog)
    {
        var runtimeDirectory = Path.GetDirectoryName(typeof(object).Assembly.Location);
        if (string.IsNullOrWhiteSpace(runtimeDirectory))
        {
            onWriteLog?.Invoke(AppLevel.R_Dll, LogLevel.Warn, "[MISS]: Не найден путь assembly для typeof(object)");
            return Array.Empty<string>();
        }

        var runtimeDirectoryPaths = Directory.GetFiles(runtimeDirectory);

        var result = PathFilter.FilterPaths(runtimeDirectoryPaths, semanticFilters.RuntimeAssemblyFilenamePatterns, (theAssembly) => (theAssembly));

        var includedLogValue = result.Any() ? string.Join("\nIncluded Runtime directory assembly: ", result.Select(a => a)) : "none";
        onWriteLog?.Invoke(AppLevel.R_Dll, LogLevel.Trace, $"Included Runtime directory assembly: {includedLogValue}");
        return result;
    }
}
