using LoggerKit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoslynKit.Model;

namespace RoslynKit.Loader;

// context: csharp, build
public static class CompilationBuilder
{
    // context: csharp, build
    public static Dictionary<SyntaxTree, SemanticModel> BuildModels(IEnumerable<string> codeFiles, RoslynCodeParserOptions options, OnWriteLog? onWriteLog = null, CancellationToken cancellationToken = default)
    {
        // 1. Читаем все файлы и создаём деревья с путями
        var syntaxTrees = codeFiles
            .Select(filePath =>
                CSharpSyntaxTree.ParseText(
                    File.ReadAllText(filePath),
                    path: filePath,
                    cancellationToken: cancellationToken))
            .ToList();

        // 2. Создаём единый Compilation
        var compilation = Build(syntaxTrees, options.CustomAssembliesPaths);

        // 3. Формируем модель для каждого дерева
        var result = new Dictionary<SyntaxTree, SemanticModel>();
        foreach(var tree in syntaxTrees)
        {
            var model = compilation.GetSemanticModel(tree);
            result[tree] = model;
        }

        return result;
    }

    // context: csharp, build
    public static CSharpCompilation Build(SyntaxTree syntaxTree, IEnumerable<string> customAssembliesPaths, string name = "Parser")
    {
        return CSharpCompilation.Create(name)
                    .AddSyntaxTrees(syntaxTree)
                    .AddReferences(AssemblyLoader.Fetch())
                    .AddReferences(AssemblyLoader.Fetch(customAssembliesPaths));
    }

    // context: csharp, build
    public static CSharpCompilation Build(IEnumerable<SyntaxTree> syntaxTrees, IEnumerable<string> customAssembliesPaths, string name = "Parser")
    {
        return CSharpCompilation.Create(name)
                    .AddSyntaxTrees(syntaxTrees)
                    .AddReferences(AssemblyLoader.Fetch())
                    .AddReferences(AssemblyLoader.Fetch(customAssembliesPaths));
    }
}