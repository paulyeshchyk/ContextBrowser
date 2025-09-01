using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ContextBrowserKit.Log;
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
        // 1. Читаем все файлы и создаём деревья с путями
        var syntaxTrees = codeFiles
            .Select(filePath => CSharpSyntaxTree.ParseText(PsoudoCodeInject(options, filePath), path: filePath, cancellationToken: cancellationToken))
            .Select(t => new RoslynSyntaxTreeWrapper(t))
            .ToList();

        // 2. Создаём единый Compilation
        var compilation = BuildCompilation(options, syntaxTrees, options.CustomAssembliesPaths);

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
    public ICompilationWrapper BuildCompilation(SemanticOptions options, IEnumerable<ISyntaxTreeWrapper> syntaxTrees, IEnumerable<string> customAssembliesPaths, string name = "Parser")
    {
        var referencesToLoad = RoslynAssemblyFetcher.Fetch(options.SemanticFilters, _logger.WriteLog);

        var compilationOptions = new CSharpCompilationOptions(
            OutputKind.DynamicallyLinkedLibrary,
            nullableContextOptions: NullableContextOptions.Enable,
            usings: new[] {
                "System",
                //"System.Diagnostics",
                //"System.Diagnostics.Process",
                //"System.Collections",
                //"System.Collections.Immutable",
                //"System.Collections.Generic",
                //"System.IO",
                //"System.Linq",
                //"System.Net.Http",
                //"System.Threading",
                "System.Threading.Tasks",
            });
        var compilation = CSharpCompilation.Create(name, options: compilationOptions)
                    .AddSyntaxTrees(syntaxTrees.Select(st => st.Tree).Cast<SyntaxTree>())
                    .AddReferences(referencesToLoad);

        var references = compilation.References;
        foreach (var reference in references)
        {
            _logger.WriteLog(AppLevel.R_Dll, LogLevel.Trace, $"Loaded reference: {reference.Display}");
        }

        var diagnostics = compilation.GetDiagnostics();
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

        return new RoslynCompilationWrapper(compilation);
    }

    private string PsoudoCodeInject(SemanticOptions _options, string filePath)
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