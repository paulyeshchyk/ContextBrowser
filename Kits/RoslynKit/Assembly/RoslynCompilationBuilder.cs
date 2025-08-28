using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoslynKit.Wrappers.Meta;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Assembly;

// context: roslyn, build
public class RoslynCompilationBuilder : ICompilationBuilder
{
    private readonly SemanticOptions _options;
    private readonly OnWriteLog? _onWriteLog;

    public RoslynCompilationBuilder(SemanticOptions options, OnWriteLog? onWriteLog)
    {
        _options = options;
        _onWriteLog = onWriteLog;
    }

    // context: roslyn, build
    public SemanticCompilationMap BuildCompilationMap(IEnumerable<string> codeFiles, CancellationToken cancellationToken = default)
    {
        // 1. Читаем все файлы и создаём деревья с путями
        var syntaxTrees = codeFiles
            .Select(filePath => CSharpSyntaxTree.ParseText(PsoudoCodeInject(filePath), path: filePath, cancellationToken: cancellationToken))
            .Select(t => new RoslynSyntaxTreeWrapper(t))
            .ToList();

        // 2. Создаём единый Compilation
        var compilation = BuildCompilation(syntaxTrees, _options.CustomAssembliesPaths);

        // 3. Формируем модель для каждого дерева
        var result = new SemanticCompilationMap();
        foreach (var tree in syntaxTrees)
        {
            var model = compilation.GetSemanticModel(tree);
            var compilationMap = new CompilationMap(tree, model);
            result.Add(compilationMap);
        }

        return result;
    }

    // context: roslyn, build
    public ICompilationWrapper BuildCompilation(IEnumerable<ISyntaxTreeWrapper> syntaxTrees, IEnumerable<string> customAssembliesPaths, string name = "Parser")
    {
        var referencesToLoad = RoslynAssemblyLoader.Fetch(_options.SemanticFilters, _onWriteLog);

        var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable);
        var compilation = CSharpCompilation.Create(name, options: compilationOptions)
                    .AddSyntaxTrees(syntaxTrees.Select(st => st.Tree).Cast<SyntaxTree>())
                    .AddReferences(referencesToLoad);

        var references = compilation.References;
        foreach (var reference in references)
        {
            _onWriteLog?.Invoke(AppLevel.R_Assembly, LogLevel.Trace, $"Loaded reference: {reference.Display}");
        }

        var diagnostics = compilation.GetDiagnostics();
        var diagnosticErrors = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error);
        foreach (var diagnostic in diagnosticErrors)
        {
            _onWriteLog?.Invoke(AppLevel.R_Assembly, LogLevel.Err, $"Diagnostics: {diagnostic}");
        }

        var diagnosticWarnings = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Warning);
        foreach (var diagnostic in diagnosticWarnings)
        {
            _onWriteLog?.Invoke(AppLevel.R_Assembly, LogLevel.Warn, $"Diagnostics: {diagnostic}");
        }

        return new RoslynCompilationWrapper(compilation);
    }

    private string PsoudoCodeInject(string filePath)
    {
        var code = File.ReadAllText(filePath);
        if (!_options.IncludePseudoCode)
        {
            return code;
        }

        if (!code.Contains("using System;", StringComparison.Ordinal))
        {
            // Вставим в самое начало, добавим отступ
            code = "using System;\n" + code;
        }
        return code;
    }
}