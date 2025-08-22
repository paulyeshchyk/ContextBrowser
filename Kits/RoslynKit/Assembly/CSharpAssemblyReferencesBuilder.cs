using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using Microsoft.CodeAnalysis;

namespace RoslynKit.Assembly;

// context: roslyn, build
public static class CSharpAssemblyReferencesBuilder
{
    // context: roslyn, build
    public static void AddTrustedAssemblies(List<PortableExecutableReference> references, OnWriteLog? onWriteLog)
    {
        //add System.Private.CoreLib
        references.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));

        var trustedAssembliesPaths = (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string ?? string.Empty)?.Split(';');
        if (trustedAssembliesPaths == null || trustedAssembliesPaths.Length == 0)
        {
            onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, "Не удалось найти доверенные сборки платформы.");
            return;
        }

        foreach (var path in trustedAssembliesPaths)
        {
            try
            {
                references.Add(MetadataReference.CreateFromFile(path));
            }
            catch (Exception ex)
            {
                onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Exception, $"Ошибка при загрузке сборки {path}: {ex.Message}");
            }
        }
    }

    public static void FetchAllLoadedAssemblies(List<PortableExecutableReference> references, OnWriteLog? onWriteLog)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
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
                onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Exception, $"Ошибка при загрузке сборки {assembly.FullName}: {ex.Message}");
            }
        }
    }
}
