using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using LoggerKit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoslynKit.Model.Meta;
using RoslynKit.Wrappers.Meta;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Assembly;

// context: roslyn, build
public class RoslynCompilationBuilder : ICompilationBuilder<RoslynSyntaxTreeWrapper>
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IAssemblyFetcher<MetadataReference> _assemblyFetcher;
    private readonly ISyntaxCompiler<MetadataReference, RoslynSyntaxTreeWrapper, CSharpCompilation> _compiler;
    private readonly ICompilationDiagnosticsInspector<CSharpCompilation> _diagnosticsInspector;

    public RoslynCompilationBuilder(
        IAppLogger<AppLevel> logger,
        ISyntaxCompiler<MetadataReference, RoslynSyntaxTreeWrapper, CSharpCompilation> compiler,
        ICompilationDiagnosticsInspector<CSharpCompilation> diagnosticsInspector,
        IAssemblyFetcher<MetadataReference> assemblyFetcher)
    {
        _logger = logger;
        _compiler = compiler;
        _diagnosticsInspector = diagnosticsInspector;
        _assemblyFetcher = assemblyFetcher;

    }

    // context: roslyn, build
    public ICompilationWrapper Build(SemanticOptions options, IEnumerable<RoslynSyntaxTreeWrapper> syntaxTrees, IEnumerable<string> customAssembliesPaths, string name, CancellationToken cancellationToken)
    {
        var assemblies = _assemblyFetcher.Fetch(options.SemanticFilters);

        var compilation = _compiler.CreateCompilation(options, syntaxTrees, name, assemblies);

        _diagnosticsInspector.LogAndFilterDiagnostics(compilation, cancellationToken);

        return new RoslynCompilationWrapper(compilation);
    }
}