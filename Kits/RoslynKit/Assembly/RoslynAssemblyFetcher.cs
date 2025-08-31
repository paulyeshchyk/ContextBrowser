using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Runtime.Serialization;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
        var trustedPlatformPaths = RoslynAssemblyFetcher.FetchTrustedPlatformPaths(semanticFilters, onWriteLog);

        var uniqAssemlyPaths = new HashSet<string>();
        uniqAssemlyPaths.UnionWith(runtimeDirectoryPaths);
        uniqAssemlyPaths.UnionWith(currentDomainPaths);

        // uniqAssemlyPaths.UnionWith(new List<string>() { "/usr/local/share/dotnet/sdk/6.0.417/System.CodeDom.dll" });

        // uniqAssemlyPaths.UnionWith(trustedPlatformPaths);
        // uniqAssemlyPaths.Add("/usr/local/share/dotnet/shared/Microsoft.NETCore.App/6.0.25/mscorlib.dll");
        // uniqAssemlyPaths.Add("/usr/local/share/dotnet/shared/Microsoft.NETCore.App/6.0.25/System.Core.dll");
        // uniqAssemlyPaths.Add("/usr/local/share/dotnet/shared/Microsoft.NETCore.App/6.0.25/System.Collections.dll");
        // uniqAssemlyPaths.Add("/Users/paul/projects/ContextBrowser/bin/Debug/net6.0/System.Collections.Immutable.dll");
        // uniqAssemlyPaths.Add("/Users/paul/projects/ContextBrowser/bin/Debug/net6.0/System.Reflection.Metadata.dll");
        // uniqAssemlyPaths.Add("/Users/paul/projects/ContextBrowser/bin/Debug/net6.0/System.Text.Encoding.CodePages.dll");
        // uniqAssemlyPaths.Add("/usr/local/share/dotnet/shared/Microsoft.NETCore.App/6.0.25/System.Runtime.dll");
        // uniqAssemlyPaths.Add(typeof(System.Net.NetworkInformation.GatewayIPAddressInformation).Assembly.Location);
        // uniqAssemlyPaths.Add(typeof(CSharpCommandLineArguments).Assembly.Location);
        // uniqAssemlyPaths.Add(typeof(SyntaxNode).Assembly.Location);
        // uniqAssemlyPaths.Add(typeof(ISymbol).Assembly.Location);
        // uniqAssemlyPaths.Add(typeof(Regex).Assembly.Location);
        // uniqAssemlyPaths.Add(typeof(JsonArray).Assembly.Location);
        // uniqAssemlyPaths.Add(typeof(CallSite).Assembly.Location);
        // uniqAssemlyPaths.Add(typeof(LinkedListNode<>).Assembly.Location);
        // uniqAssemlyPaths.Add(typeof(Console).Assembly.Location);
        // uniqAssemlyPaths.Add(typeof(ProcessStartInfo).Assembly.Location);
        // uniqAssemlyPaths.Add(typeof(ConcurrentDictionary<,>).Assembly.Location);


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
        var result = PathFilter.FilterPaths(runtimeDirectoryPaths, semanticFilters.RuntimeAssemblyFilenamePatterns, (p) => p);
        var includedLogValue = result.Any() ? string.Join("\nIncluded Runtime directory assembly: ", result.Select(a => a)) : "none";
        onWriteLog?.Invoke(AppLevel.R_Dll, LogLevel.Trace, $"Included Runtime directory assembly: {includedLogValue}");
        return result;
    }
}
