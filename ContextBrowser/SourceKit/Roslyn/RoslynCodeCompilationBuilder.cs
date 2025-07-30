using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ContextBrowser.SourceKit.Roslyn;

// context: csharp, build
public static class RoslynCodeCompilationBuilder
{
    // context: csharp, build
    public static Dictionary<SyntaxTree, SemanticModel> BuildModels(IEnumerable<string> codeFiles, RoslynCodeParserOptions options)
    {
        // 1. Читаем все файлы и создаём деревья с путями
        var syntaxTrees = codeFiles
            .Select(filePath =>
                CSharpSyntaxTree.ParseText(
                    File.ReadAllText(filePath),
                    path: filePath,
                    cancellationToken: CancellationToken.None))
            .ToList();

        // 2. Создаём единый Compilation
        var compilation = RoslynCodeCompilationBuilder.Build(syntaxTrees, options.CustomAssembliesPaths);

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
                    .AddReferences(RoslynCodeParserAssemblyReferencesBuilder.CSharpCompilationReferences())
                    .AddReferences(RoslynCodeParserAssemblyReferencesBuilder.CSharpCompilationReferences(customAssembliesPaths));
    }

    // context: csharp, build
    public static CSharpCompilation Build(IEnumerable<SyntaxTree> syntaxTrees, IEnumerable<string> customAssembliesPaths, string name = "Parser")
    {
        return CSharpCompilation.Create(name)
                    .AddSyntaxTrees(syntaxTrees)
                    .AddReferences(RoslynCodeParserAssemblyReferencesBuilder.CSharpCompilationReferences())
                    .AddReferences(RoslynCodeParserAssemblyReferencesBuilder.CSharpCompilationReferences(customAssembliesPaths));
    }
}