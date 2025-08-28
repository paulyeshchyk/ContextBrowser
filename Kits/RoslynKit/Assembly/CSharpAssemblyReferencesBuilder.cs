using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using Microsoft.CodeAnalysis;

namespace RoslynKit.Assembly;

// context: roslyn, build
public static class CSharpAssemblyReferencesBuilder
{
    // context: roslyn, build
    public static void AddTrustedAssemblies(string excludedAssemblyNamesPattern, List<PortableExecutableReference> references, OnWriteLog? onWriteLog)
    {
        //add System.Private.CoreLib
        references.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));

        references.Add(MetadataReference.CreateFromFile(typeof(Attribute).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(CancellationToken).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(Dictionary<,>).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(LinkedListNode<>).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(LinkedList<>).Assembly.Location));

        references.Add(MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(IEnumerable<>).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(KeyValuePair<,>).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(Exception).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(File).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(HashSet<>).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(List<>).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(Stream).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(System.Action).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(System.Attribute).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(System.Exception).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(System.IO.FileStream).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(System.Runtime.InteropServices.Marshal).Assembly.Location)); // Important for many base types
        references.Add(MetadataReference.CreateFromFile(typeof(System.Threading.Tasks.Task).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(Task).Assembly.Location));

        var trustedAssembliesPaths = (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string ?? string.Empty).Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);
        if (trustedAssembliesPaths == null || trustedAssembliesPaths.Length == 0)
        {
            onWriteLog?.Invoke(AppLevel.R_Assembly, LogLevel.Warn, "Не удалось найти доверенные сборки платформы.");
            return;
        }

        var excluded = PathFilter.FilterPaths(trustedAssembliesPaths, excludedAssemblyNamesPattern, (thePath) => (thePath));
        var filtered = PathFilter.FilteroutPaths(trustedAssembliesPaths, excludedAssemblyNamesPattern, (thePath) => (thePath));

        var excludedLogValue = excluded.Any() ? string.Join("\nExcluded assembly: ", excluded) : "none";
        onWriteLog?.Invoke(AppLevel.R_Assembly, LogLevel.Trace, $"Excluded assembly: {excludedLogValue}");

        var includedLogValue = filtered.Any() ? string.Join("\nIncluded assembly: ", filtered) : "none";
        onWriteLog?.Invoke(AppLevel.R_Assembly, LogLevel.Trace, $"Included assembly: {includedLogValue}");

        foreach (var path in filtered)
        {
            try
            {
                references.Add(MetadataReference.CreateFromFile(path));
            }
            catch (Exception ex)
            {
                onWriteLog?.Invoke(AppLevel.R_Assembly, LogLevel.Exception, $"Ошибка при загрузке сборки {path}: {ex.Message}");
            }
        }
    }

    public static void FetchAllLoadedAssemblies(string excludedAssemblyNamesPattern, List<PortableExecutableReference> references, OnWriteLog? onWriteLog)
    {
        var domainAssemblies = AppDomain.CurrentDomain.GetAssemblies();

        var excluded = PathFilter.FilterPaths(domainAssemblies, excludedAssemblyNamesPattern, (theAssembly) => (theAssembly.Location));
        var filtered = PathFilter.FilteroutPaths(domainAssemblies, excludedAssemblyNamesPattern, (theAssembly) => (theAssembly.Location));


        foreach (var assembly in filtered)
        {
            try
            {
                if (!assembly.IsDynamic)
                {
                    references.Add(MetadataReference.CreateFromFile(assembly.Location));
                }
            }
            catch (Exception ex)
            {
                onWriteLog?.Invoke(AppLevel.R_Assembly, LogLevel.Exception, $"Ошибка при загрузке сборки {assembly.FullName}: {ex.Message}");
            }
        }
    }
}
