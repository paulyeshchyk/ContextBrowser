using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using LoggerKit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoslynKit.Wrappers.Meta;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Assembly;

// context: roslyn, build
public class RoslynCompilationBuilder : ICompilationBuilder
{
    private readonly IAppLogger<AppLevel> _logger;

    public RoslynCompilationBuilder(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    // context: roslyn, build
    public SemanticCompilationMap BuildCompilationMap(SemanticOptions options, IEnumerable<string> codeFiles, CancellationToken cancellationToken = default)
    {
        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, "Phase 1: Build compilation map", LogLevelNode.Start);

        // 1. Читаем все файлы и создаём деревья с путями
        var syntaxTrees = codeFiles
            .Select(filePath => CSharpSyntaxTree.ParseText(PsoudoCodeInject(options, filePath), path: filePath, cancellationToken: cancellationToken))
            .Select(t => new RoslynSyntaxTreeWrapper(t))
            .ToList();

        // 2. Создаём единый Compilation
        var compilation = BuildCompilation(options, syntaxTrees, options.CustomAssembliesPaths);

        // 3. Формируем модель для каждого дерева
        var result = BuildSemanticCompilationMap(syntaxTrees, compilation);

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, "Phase 1: Build compilation map", LogLevelNode.End);

        return result;
    }

    private SemanticCompilationMap BuildSemanticCompilationMap(List<RoslynSyntaxTreeWrapper> syntaxTrees, ICompilationWrapper compilation)
    {
        _logger.WriteLog(AppLevel.R_Dll, LogLevel.Cntx, $"Compilation map building", LogLevelNode.Start);
        var result = new SemanticCompilationMap();
        foreach (var tree in syntaxTrees)
        {
            var compilationMap = BuildCompilationMap(compilation, tree);
            result.Add(compilationMap);
        }
        _logger.WriteLog(AppLevel.R_Dll, LogLevel.Cntx, $"Compilation map build done", LogLevelNode.End);
        return result;
    }

    private CompilationMap BuildCompilationMap(ICompilationWrapper compilation, RoslynSyntaxTreeWrapper tree)
    {
        _logger.WriteLog(AppLevel.R_Dll, LogLevel.Trace, $"Compilation map building for: {tree.FilePath}");
        var model = compilation.GetSemanticModel(tree);
        var compilationMap = new CompilationMap(tree, model);
        return compilationMap;
    }

    // context: roslyn, build
    public ICompilationWrapper BuildCompilation(SemanticOptions options, IEnumerable<ISyntaxTreeWrapper> syntaxTrees, IEnumerable<string> customAssembliesPaths, string name = "Parser")
    {
        var referencesToLoad = RoslynAssemblyFetcher.Fetch(options.SemanticFilters, _logger);
        var usings = GetValidatedUsingsFromOptions(options);
        var compilationOptions = new CSharpCompilationOptions
        (
            OutputKind.DynamicallyLinkedLibrary,
            nullableContextOptions: NullableContextOptions.Enable,
            usings: usings);

        _logger.WriteLog(AppLevel.R_Dll, LogLevel.Cntx, $"Compilation loading", LogLevelNode.Start);
        var compilation = CSharpCompilation.Create(name, options: compilationOptions)
                    .AddSyntaxTrees(syntaxTrees.Select(st => st.Tree).Cast<SyntaxTree>())
                    .AddReferences(referencesToLoad);

        var references = compilation.References;
        foreach (var reference in references)
        {
            _logger.WriteLog(AppLevel.R_Dll, LogLevel.Trace, $"Loaded reference: {reference.Display}");
        }

        var diagnostics = compilation.GetDiagnostics().ToList();
        var diagnosticErrors = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error);
        foreach (var diagnostic in diagnosticErrors)
        {
            _logger.WriteLog(AppLevel.R_Dll, LogLevel.Err, $"Diagnostics: {diagnostic}");
        }

        var diagnosticWarnings = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Warning);
        foreach (var diagnostic in diagnosticWarnings)
        {
            _logger.WriteLog(AppLevel.R_Dll, LogLevel.Trace, $"Diagnostics: {diagnostic}");
        }
        _logger.WriteLog(AppLevel.R_Dll, LogLevel.Cntx, $"Compilation loaded", LogLevelNode.End);

        return new RoslynCompilationWrapper(compilation);
    }

    private static string[] GetValidatedUsingsFromOptions(SemanticOptions options)
    {
        string[] SDefaultUsing = new string[] { "System" };

        var result = string.IsNullOrWhiteSpace(options.GlobalUsings)
            ? SDefaultUsing
            : options.GlobalUsings.Split(";").Select(s => s.Trim()).ToArray();
        return (result.Length == 0)
            ? SDefaultUsing
            : result;
    }

    private static string PsoudoCodeInject(SemanticOptions _options, string filePath)
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