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
public class RoslynCompilationMapBuilder : ISemanticMapExtractor<RoslynSyntaxTreeWrapper>
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly ISyntaxTreeParser<RoslynSyntaxTreeWrapper> _syntaxTreeParser;
    private readonly ICompilationMapMapper<RoslynSyntaxTreeWrapper> _compilationMapMapper;
    private readonly ICompilationBuilder<RoslynSyntaxTreeWrapper> _compilationMapBuilder;

    public RoslynCompilationMapBuilder(
        IAppLogger<AppLevel> logger,
        ISyntaxTreeParser<RoslynSyntaxTreeWrapper> syntaxTreeParser,
        ICompilationMapMapper<RoslynSyntaxTreeWrapper> compilationMapMapper,
        ICompilationBuilder<RoslynSyntaxTreeWrapper> compilationMapBuilder)
    {
        _logger = logger;
        _syntaxTreeParser = syntaxTreeParser;
        _compilationMapMapper = compilationMapMapper;
        _compilationMapBuilder = compilationMapBuilder;
    }

    // context: roslyn, build
    public async Task<SemanticCompilationMap<RoslynSyntaxTreeWrapper>> CreateSemanticMapFromFilesAsync(SemanticOptions options, IEnumerable<string> codeFiles, CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, "Phase 1: Build compilation map", LogLevelNode.Start);

        var result = await CreateSyntaxTreesAndMapAsync(options, codeFiles, cancellationToken).ConfigureAwait(false);

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, "Phase 1: Build compilation map", LogLevelNode.End);
        return result;
    }

    // context: roslyn, build
    public Task<SemanticCompilationMap<RoslynSyntaxTreeWrapper>> CreateSemanticMapFromFilesAsync(SemanticOptions options, string filePath, CancellationToken cancellationToken)
    {
        var codeFiles = new[] { filePath };

        return CreateSemanticMapFromFilesAsync(options, codeFiles, cancellationToken);
    }

    // context: roslyn, build
    internal async Task<SemanticCompilationMap<RoslynSyntaxTreeWrapper>> CreateSyntaxTreesAndMapAsync(SemanticOptions options, IEnumerable<string> codeFiles, CancellationToken cancellationToken)
    {
        var syntaxTrees = await _syntaxTreeParser.ParseFilesToSyntaxTreesAsync(options, codeFiles, cancellationToken);

        var compilation = _compilationMapBuilder.Build(options, syntaxTrees, options.CustomAssembliesPaths, "Parser", cancellationToken);

        var result = _compilationMapMapper.MapSemanticModelToCompilationMap(syntaxTrees, compilation);

        return result;
    }
}