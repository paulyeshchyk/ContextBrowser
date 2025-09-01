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
using ContextBrowserKit.Filters;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using LoggerKit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Hosting;
using SemanticKit.Model.Options;

namespace RoslynKit.Assembly;

// context: roslyn, build
public static class RoslynAssemblyFetcher
{
    // context: roslyn, build
    public static IEnumerable<MetadataReference> Fetch(AssemblyPathFilterPatterns assemblyPathsFilter, IAppLogger<AppLevel> logger)
    {
        var trustedPlatformPaths = RoslynAssemblyFetcher.FetchTrustedPlatformPaths(logger);
        var filteredTrustedPaths = RoslynAssemblyFetcher.ApplyFilters(trustedPlatformPaths, assemblyPathsFilter.TrustedFilters);
        logger.WriteLog(AppLevel.R_Dll, LogLevel.Cntx, $"Загрузится {filteredTrustedPaths.Count()} trusted сборок");

        var currentDomainPaths = RoslynAssemblyFetcher.FetchCurrentDomainPaths(logger);
        var filteredDomainPaths = RoslynAssemblyFetcher.ApplyFilters(currentDomainPaths, assemblyPathsFilter.DomainFilters);
        logger.WriteLog(AppLevel.R_Dll, LogLevel.Cntx, $"Загрузится {filteredDomainPaths.Count()} domain сборок");

        var runtimeDirectoryPaths = RoslynAssemblyFetcher.FetchRuntimeDirectoryAssemblyPaths(logger);
        var filteredRuntimePaths = RoslynAssemblyFetcher.ApplyFilters(runtimeDirectoryPaths, assemblyPathsFilter.DomainFilters);
        logger.WriteLog(AppLevel.R_Dll, LogLevel.Cntx, $"Загрузится {filteredRuntimePaths.Count()} domain сборок");

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
    private static string[] FetchTrustedPlatformPaths(IAppLogger<AppLevel> logger)
    {
        var result = (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string ?? string.Empty)
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);

        if (result.Length == 0)
        {
            logger.WriteLog(AppLevel.R_Dll, LogLevel.Warn, "Не удалось найти доверенные сборки платформы.");
            return Array.Empty<string>();
        }

        return result;
    }

    // context: roslyn, build
    private static string[] FetchCurrentDomainPaths(IAppLogger<AppLevel> logger)
    {
        var domainAssemblies = AssemblyLoadContext.Default.Assemblies.ToArray();
        var result = new List<string>();

        foreach (var assembly in domainAssemblies)
        {
            try
            {
                if (assembly.IsDynamic)
                {
                    logger.WriteLog(AppLevel.R_Dll, LogLevel.Trace, $"[SKIP] Dynamic assembly {assembly.FullName}");
                }
                else
                {
                    result.Add(assembly.Location);
                }
            }
            catch (Exception ex)
            {
                logger.WriteLog(AppLevel.R_Dll, LogLevel.Exception, $"Ошибка при загрузке сборки {assembly.FullName}: {ex.Message}");
            }
        }

        return result.ToArray();
    }

    // context: roslyn, build
    private static string[] FetchRuntimeDirectoryAssemblyPaths(IAppLogger<AppLevel> logger)
    {
        var runtimeDirectory = Path.GetDirectoryName(typeof(object).Assembly.Location);
        if (string.IsNullOrWhiteSpace(runtimeDirectory))
        {
            logger.WriteLog(AppLevel.R_Dll, LogLevel.Warn, "[MISS]: Не найден путь assembly для typeof(object)");
            return Array.Empty<string>();
        }

        var result = Directory.GetFiles(runtimeDirectory, "*.dll", SearchOption.TopDirectoryOnly).ToList();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            result.AddRange(Directory.GetFiles(runtimeDirectory, "*.exe", SearchOption.TopDirectoryOnly));
        }

        return result.ToArray();
    }

    internal static IEnumerable<string> ApplyFilters(string[] paths, FilterPatterns filterPatterns)
    {
        // из всего списка оставляем только те, что нужны
        var filteredInPaths = PathFilter.FilterPaths(items: paths, filter: filterPatterns.Included, pathSelector: (thePath) => thePath);

        // из оставшегося списка убираем те, что не нужны
        return PathFilter.FilteroutPaths(items: filteredInPaths, filter: filterPatterns.Excluded, pathSelector: (thePath) => thePath);
    }
}