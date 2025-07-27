using Microsoft.CodeAnalysis;

namespace ContextBrowser.SourceKit.Roslyn;

public static class RoslynCodeParserAssemblyReferencesBuilder
{
    public static IEnumerable<MetadataReference> CSharpCompilationReferences()
    {
        var runtimeDirectory = Path.GetDirectoryName(typeof(object).Assembly.Location);
        if(string.IsNullOrWhiteSpace(runtimeDirectory))
        {
            Console.WriteLine("Не найден путь assembly для typeof(object)");
            return Enumerable.Empty<MetadataReference>();
        }

        // Собираем все .dll файлы из этой директории как ссылки
        // (Можно фильтровать по имени, чтобы не добавлять все подряд, но для общего парсинга это хороший старт)
        var references = Directory.GetFiles(runtimeDirectory, "*.dll")
                                  .Select(path => MetadataReference.CreateFromFile(path))
                                  .ToList();

        // Дополнительные, часто используемые ссылки, если их нет в runtimeDirectory
        // (иногда некоторые сборки находятся в других местах или их нужно явно добавить)
        references.Add(MetadataReference.CreateFromFile(typeof(System.IO.StringWriter).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(System.IO.TextWriter).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(System.Console).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(System.Collections.Generic.List<>).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(System.Net.Http.HttpClient).Assembly.Location)); // Для HTTP-запросов
        references.Add(MetadataReference.CreateFromFile(typeof(System.Text.RegularExpressions.Regex).Assembly.Location)); // Для Regex
        references.Add(MetadataReference.CreateFromFile(typeof(System.Text.Json.JsonSerializer).Assembly.Location)); // Для System.Text.Json
        references.Add(MetadataReference.CreateFromFile(typeof(System.Xml.Linq.XDocument).Assembly.Location)); // Для XML LINQ
        references.Add(MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location)); // Для LINQ
        references.Add(MetadataReference.CreateFromFile(typeof(System.Net.WebUtility).Assembly.Location)); // Для WebUtility
        return references;
    }

    public static IEnumerable<MetadataReference> CSharpCompilationReferences(IEnumerable<string> runtimeDirectory)
    {
        var references = new List<MetadataReference>();
        foreach(var directory in runtimeDirectory)
        {
            references.AddRange(CSharpCompilationReferences(directory));
        }
        return references;
    }

    public static IEnumerable<MetadataReference> CSharpCompilationReferences(string runtimeDirectory)
    {
        if(string.IsNullOrWhiteSpace(runtimeDirectory) || !Directory.Exists(runtimeDirectory))
        {
            Console.WriteLine($"Warning: Custom project bin path not found or invalid: {runtimeDirectory}");
            return Enumerable.Empty<MetadataReference>();
        }

        var references = new List<MetadataReference>();
        foreach(var file in Directory.EnumerateFiles(runtimeDirectory, "*.dll"))
        {
            try
            {
                references.Add(MetadataReference.CreateFromFile(file));
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Warning: Could not add reference from {file}: {ex.Message}");
            }
        }
        return references;
    }
}
