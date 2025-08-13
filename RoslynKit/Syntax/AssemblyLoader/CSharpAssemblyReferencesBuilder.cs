using Microsoft.CodeAnalysis;

namespace RoslynKit.Syntax.AssemblyLoader;

// context: csharp, build
public static class CSharpAssemblyReferencesBuilder
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

    public static void FetchAllLoadedAssemblies(List<PortableExecutableReference> references)
    {
        foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                if(!assembly.IsDynamic)
                {
                    references.Add(MetadataReference.CreateFromFile(assembly.Location));
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке сборки {assembly.FullName}: {ex.Message}");
            }
        }
    }
}
