using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Filters;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using LoggerKit;
using Microsoft.CodeAnalysis;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Assembly;

// context: roslyn, build
// coverage: 50
public class RoslynAssemblyFetcher : IAssemblyFetcher<MetadataReference>
{
    private readonly IAppLogger<AppLevel> _logger;

    public RoslynAssemblyFetcher(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    // context: assembly, read
    // coverage: 20
    public IEnumerable<MetadataReference> Fetch(AssemblyPathFilterPatterns assemblyPathsFilter)
    {
        var trustedPlatformPaths = FetchTrustedPlatformPaths();
        var filteredTrustedPaths = ApplyFilters(trustedPlatformPaths, assemblyPathsFilter.TrustedFilters);
        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Cntx, $"Загрузится {filteredTrustedPaths.Count()} trusted сборок");

        var currentDomainPaths = FetchCurrentDomainPaths();
        var filteredDomainPaths = ApplyFilters(currentDomainPaths, assemblyPathsFilter.DomainFilters);
        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Cntx, $"Загрузится {filteredDomainPaths.Count()} domain сборок");

        var runtimeDirectoryPaths = FetchRuntimeDirectoryAssemblyPaths();
        var filteredRuntimePaths = ApplyFilters(runtimeDirectoryPaths, assemblyPathsFilter.DomainFilters);
        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Cntx, $"Загрузится {filteredRuntimePaths.Count()} domain сборок");

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
    internal string[] FetchTrustedPlatformPaths()
    {
        var result = (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string ?? string.Empty)
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);

        if (result.Length == 0)
        {
            _logger.WriteLog(AppLevel.R_Dll, LogLevel.Warn, "Не удалось найти доверенные сборки платформы.");
            return [];
        }

        return result;
    }

    // context: roslyn, build
    internal string[] FetchCurrentDomainPaths()
    {
        var domainAssemblies = AssemblyLoadContext.Default.Assemblies.ToArray();
        var result = new List<string>();

        foreach (var assembly in domainAssemblies)
        {
            try
            {
                if (assembly.IsDynamic)
                {
                    _logger.WriteLog(AppLevel.R_Dll, LogLevel.Trace, $"[SKIP] Dynamic assembly {assembly.FullName}");
                }
                else
                {
                    result.Add(assembly.Location);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLog(AppLevel.R_Dll, LogLevel.Exception, $"Ошибка при загрузке сборки {assembly.FullName}: {ex.Message}");
            }
        }

        return result.ToArray();
    }

    // context: roslyn, build
    internal string[] FetchRuntimeDirectoryAssemblyPaths()
    {
        var runtimeDirectory = Path.GetDirectoryName(typeof(object).Assembly.Location);
        if (string.IsNullOrWhiteSpace(runtimeDirectory))
        {
            _logger.WriteLog(AppLevel.R_Dll, LogLevel.Warn, "[MISS]: Не найден путь assembly для typeof(object)");
            return [];
        }

        var result = Directory.GetFiles(runtimeDirectory, "*.dll", SearchOption.TopDirectoryOnly).ToList();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            result.AddRange(Directory.GetFiles(runtimeDirectory, "*.exe", SearchOption.TopDirectoryOnly));
        }

        return result.ToArray();
    }

    // context: roslyn, build
    internal static IEnumerable<string> ApplyFilters(string[] paths, FilterPatterns filterPatterns)
    {
        // из всего списка оставляем только те, что нужны
        var filteredInPaths = PathFilter.FilterPaths(items: paths, filter: filterPatterns.Included, pathSelector: (thePath) => thePath);

        // из оставшегося списка убираем те, что не нужны
        return PathFilter.FilteroutPaths(items: filteredInPaths, filter: filterPatterns.Excluded, pathSelector: (thePath) => thePath);
    }
}