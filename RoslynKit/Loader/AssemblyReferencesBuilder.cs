using Microsoft.CodeAnalysis;

namespace RoslynKit.Loader;

// context: csharp, build
public static class AssemblyReferencesBuilder
{
    // context: csharp, build
    public static void AddTrustedAssemblies(List<PortableExecutableReference> references)
    {
        //add System.Private.CoreLib
        references.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));

        var trustedAssembliesPaths = (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string ?? string.Empty)?.Split(';');
        if(trustedAssembliesPaths == null || trustedAssembliesPaths.Length == 0)
        {
            // Обработка ошибки: не удалось найти сборки
            Console.WriteLine("Не удалось найти доверенные сборки платформы.");
            return;
        }

        foreach(var path in trustedAssembliesPaths)
        {
            try
            {
                references.Add(MetadataReference.CreateFromFile(path));
            }
            catch(Exception ex)
            {
                // Здесь можно добавить логирование, если какая-то сборка не найдена
                Console.WriteLine($"Ошибка при загрузке сборки {path}: {ex.Message}");
            }
        }
    }

    // context: csharp, build
    public static void AddExtraAssemblies(List<PortableExecutableReference> references)
    {
        // Дополнительные, часто используемые ссылки, если их нет в runtimeDirectory
        // (иногда некоторые сборки находятся в других местах или их нужно явно добавить)
        references.Add(MetadataReference.CreateFromFile(typeof(StringWriter).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(TextWriter).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(Console).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(List<>).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(HttpClient).Assembly.Location)); // Для HTTP-запросов
        references.Add(MetadataReference.CreateFromFile(typeof(System.Text.RegularExpressions.Regex).Assembly.Location)); // Для Regex
        references.Add(MetadataReference.CreateFromFile(typeof(System.Text.Json.JsonSerializer).Assembly.Location)); // Для System.Text.Json
        references.Add(MetadataReference.CreateFromFile(typeof(System.Xml.Linq.XDocument).Assembly.Location)); // Для XML LINQ
        references.Add(MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location)); // Для LINQ
        references.Add(MetadataReference.CreateFromFile(typeof(System.Net.WebUtility).Assembly.Location)); // Для WebUtility
    }
}
