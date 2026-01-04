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
public class RoslynCompilationBuilder : ICompilationBuilder
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly ISyntaxCompiler<MetadataReference> _compiler;
    private readonly ISyntaxTreeParser<RoslynSyntaxTreeWrapper> _syntaxTreeParser;
    private readonly ICompilationMapMapper<RoslynSyntaxTreeWrapper> _compilationMapMapper;
    private readonly ICompilationDiagnosticsInspector<CSharpCompilation> _diagnosticsInspector;

    public RoslynCompilationBuilder(IAppLogger<AppLevel> logger, ISyntaxCompiler<MetadataReference> compiler, ISyntaxTreeParser<RoslynSyntaxTreeWrapper> syntaxTreeParser, ICompilationMapMapper<RoslynSyntaxTreeWrapper> compilationMapMapper, ICompilationDiagnosticsInspector<CSharpCompilation> diagnosticsInspector)
    {
        _logger = logger;
        _compiler = compiler;
        _syntaxTreeParser = syntaxTreeParser;
        _compilationMapMapper = compilationMapMapper;
        _diagnosticsInspector = diagnosticsInspector;
    }

    // context: roslyn, build
    public ICompilationWrapper Build(SemanticOptions options, IEnumerable<ISyntaxTreeWrapper> syntaxTrees, IEnumerable<string> customAssembliesPaths, string name, CancellationToken cancellationToken)
    {
        var referencesToLoad = RoslynAssemblyFetcher.Fetch(options.SemanticFilters, _logger);

        var compilation = _compiler.CreateCompilation(options, syntaxTrees, name, referencesToLoad);

        _diagnosticsInspector.LogAndFilterDiagnostics(compilation, cancellationToken);

        _logger.WriteLog(AppLevel.R_Dll, LogLevel.Cntx, "Compilation loaded", LogLevelNode.End);

        return new RoslynCompilationWrapper(compilation);
    }

    // context: roslyn, build
    public async Task<SemanticCompilationMap> CreateSemanticMapFromFilesAsync(SemanticOptions options, IEnumerable<string> codeFiles, CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, "Phase 1: Build compilation map", LogLevelNode.Start);

        var result = await CreateSyntaxTreesAndMapAsync(options, codeFiles, cancellationToken).ConfigureAwait(false);

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, "Phase 1: Build compilation map", LogLevelNode.End);
        return result;
    }

    // context: roslyn, build
    public Task<SemanticCompilationMap> CreateSemanticMapFromFilesAsync(SemanticOptions options, string filePath, CancellationToken cancellationToken)
    {
        var codeFiles = new[] { filePath };

        return CreateSemanticMapFromFilesAsync(options, codeFiles, cancellationToken);
    }

    internal async Task<SemanticCompilationMap> CreateSyntaxTreesAndMapAsync(SemanticOptions options, IEnumerable<string> codeFiles, CancellationToken cancellationToken)
    {
        var syntaxTrees = await _syntaxTreeParser.ParseFilesToSyntaxTreesAsync(options, codeFiles, cancellationToken);

        var compilation = Build(options, syntaxTrees, options.CustomAssembliesPaths, "Parser", cancellationToken);

        var result = _compilationMapMapper.MapSemanticModelToCompilationMap(syntaxTrees, compilation);

        return result;
    }
}