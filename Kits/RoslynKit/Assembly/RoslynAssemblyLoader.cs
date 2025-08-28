using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using Microsoft.CodeAnalysis;
using SemanticKit.Model.Options;

namespace RoslynKit.Assembly;

// context: roslyn, build
public static class RoslynAssemblyLoader
{
    // context: roslyn, build
    public static IEnumerable<MetadataReference> Fetch(SemanticFilters semanticFilters, OnWriteLog? onWriteLog)
    {
        var runtimeDirectory = Path.GetDirectoryName(typeof(object).Assembly.Location);
        if (string.IsNullOrWhiteSpace(runtimeDirectory))
        {
            onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, "[MISS]: Не найден путь assembly для typeof(object)");
            return Enumerable.Empty<MetadataReference>();
        }

        // Собираем все .dll файлы из этой директории как ссылки
        // (Можно фильтровать по имени, чтобы не добавлять все подряд, но для общего парсинга это хороший старт)
        var references = Directory.GetFiles(runtimeDirectory, semanticFilters.RuntimeAssemblyFilenamePattern)
                                  .Select(path => MetadataReference.CreateFromFile(path))
                                  .ToList();

        CSharpAssemblyReferencesBuilder.FetchAllLoadedAssemblies(semanticFilters.ExcludedAssemblyNamesPattern, references, onWriteLog);
        CSharpAssemblyReferencesBuilder.AddTrustedAssemblies(semanticFilters.ExcludedAssemblyNamesPattern, references, onWriteLog);
        return references;
    }
}