using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using Microsoft.CodeAnalysis;

namespace RoslynKit.Assembly;

// context: roslyn, build
public static class RoslynAssemblyLoader
{
    // context: roslyn, build
    public static IEnumerable<MetadataReference> Fetch(OnWriteLog? onWriteLog)
    {
        var runtimeDirectory = Path.GetDirectoryName(typeof(object).Assembly.Location);
        if (string.IsNullOrWhiteSpace(runtimeDirectory))
        {
            onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, "[MISS]: Не найден путь assembly для typeof(object)");
            return Enumerable.Empty<MetadataReference>();
        }

        // Собираем все .dll файлы из этой директории как ссылки
        // (Можно фильтровать по имени, чтобы не добавлять все подряд, но для общего парсинга это хороший старт)
        var references = Directory.GetFiles(runtimeDirectory, "System.Runtime*.dll")
                                  .Select(path => MetadataReference.CreateFromFile(path))
                                  .ToList();

        CSharpAssemblyReferencesBuilder.FetchAllLoadedAssemblies(references, onWriteLog);
        CSharpAssemblyReferencesBuilder.AddTrustedAssemblies(references, onWriteLog);
        return references;
    }

    // context: roslyn, build
    public static IEnumerable<MetadataReference> Fetch(string runtimeDirectory, OnWriteLog? onWriteLog)
    {
        if (string.IsNullOrWhiteSpace(runtimeDirectory) || !Directory.Exists(runtimeDirectory))
        {
            onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"Warning: Custom project bin path not found or invalid: {runtimeDirectory}");

            return Enumerable.Empty<MetadataReference>();
        }

        var references = new List<MetadataReference>();
        foreach (var file in Directory.EnumerateFiles(runtimeDirectory, "*.dll"))
        {
            try
            {
                references.Add(MetadataReference.CreateFromFile(file));
            }
            catch (Exception ex)
            {
                onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Warning: Could not add reference from {file}: {ex.Message}");
            }
        }
        return references;
    }
}