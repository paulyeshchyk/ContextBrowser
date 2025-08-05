using LoggerKit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoslynKit.Model;

namespace RoslynKit.Loader;

// context: csharp, build
public static class CompilationBuilder
{
    private static string PsoudoCodeInject(string filePath)
    {
        var code = File.ReadAllText(filePath);
        if(!code.Contains("using System;", StringComparison.Ordinal))
        {
            // Вставим в самое начало, добавим отступ
            code = "using System;\n" + code;
        }
        return code;
    }

    // context: csharp, build
    public static Dictionary<SyntaxTree, SemanticModel> BuildModels(IEnumerable<string> codeFiles, RoslynCodeParserOptions options, OnWriteLog? onWriteLog = null, CancellationToken cancellationToken = default)
    {
        // 1. Читаем все файлы и создаём деревья с путями
        var syntaxTrees = codeFiles
            .Select(filePath => CSharpSyntaxTree.ParseText(PsoudoCodeInject(filePath), path: filePath, cancellationToken: cancellationToken))
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
    public static CSharpCompilation Build(IEnumerable<SyntaxTree> syntaxTrees, IEnumerable<string> customAssembliesPaths, string name = "Parser")
    {
        var compilation = CSharpCompilation.Create(name, options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                    .AddSyntaxTrees(syntaxTrees)
                    .AddReferences(AssemblyLoader.Fetch());

        var diags = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error);
        foreach(var diagnostic in diags)
        {
            Console.WriteLine(diagnostic.ToString());
        }

        return compilation;
    }
}