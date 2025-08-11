using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoslynKit.Model;

namespace RoslynKit.Loader;

// context: csharp, build
public class CompilationBuilder
{
    private readonly RoslynCodeParserOptions _options;
    private readonly OnWriteLog? _onWriteLog;

    public CompilationBuilder(RoslynCodeParserOptions options, OnWriteLog? onWriteLog)
    {
        _options = options;
        _onWriteLog = onWriteLog;
    }

    // context: csharp, build
    public Dictionary<SyntaxTree, SemanticModel> BuildModels(IEnumerable<string> codeFiles, CancellationToken cancellationToken = default)
    {
        // 1. Читаем все файлы и создаём деревья с путями
        var syntaxTrees = codeFiles
            .Select(filePath => CSharpSyntaxTree.ParseText(PsoudoCodeInject(filePath), path: filePath, cancellationToken: cancellationToken))
            .ToList();

        // 2. Создаём единый Compilation
        var compilation = Build(syntaxTrees, _options.CustomAssembliesPaths);

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
    public CSharpCompilation Build(IEnumerable<SyntaxTree> syntaxTrees, IEnumerable<string> customAssembliesPaths, string name = "Parser")
    {
        var compilation = CSharpCompilation.Create(name, options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                    .AddSyntaxTrees(syntaxTrees)
                    .AddReferences(AssemblyLoader.Fetch(_onWriteLog));

        var diags = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error);
        foreach(var diagnostic in diags)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, diagnostic.ToString());
        }

        return compilation;
    }

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
}