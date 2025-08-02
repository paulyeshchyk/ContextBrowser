using Microsoft.CodeAnalysis;

namespace RoslynKit.Loader;

// context: csharp, build
public static class AssemblyLoader
{
    // context: csharp, build
    public static IEnumerable<MetadataReference> Fetch()
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

        AssemblyReferencesBuilder.AddTrustedAssemblies(references);

        AssemblyReferencesBuilder.AddExtraAssemblies(references);

        return references;
    }


    // context: csharp, build
    public static IEnumerable<MetadataReference> Fetch(IEnumerable<string> runtimeDirectory)
    {
        var references = new List<MetadataReference>();
        foreach(var directory in runtimeDirectory)
        {
            references.AddRange(Fetch(directory));
        }
        return references;
    }

    // context: csharp, build
    public static IEnumerable<MetadataReference> Fetch(string runtimeDirectory)
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