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

    // context: roslyn, build, compilationFlow
    public async Task<SemanticCompilationMap<RoslynSyntaxTreeWrapper>> CreateSemanticMapFromFilesAsync(SemanticOptions options, IEnumerable<string> codeFiles, CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Cntx, $"Building compilation map for {codeFiles.Count()} files", LogLevelNode.None);

        var syntaxTrees = await _syntaxTreeParser.ParseFilesToSyntaxTreesAsync(options, codeFiles, cancellationToken).ConfigureAwait(false);

        var compilation = await _compilationMapBuilder.BuildAsync(options, syntaxTrees, options.CustomAssembliesPaths, "Parser", cancellationToken).ConfigureAwait(false);

        var result = _compilationMapMapper.MapSemanticModelToCompilationMap(syntaxTrees, compilation);

        return result;
    }

    // context: roslyn, build
    public Task<SemanticCompilationMap<RoslynSyntaxTreeWrapper>> CreateSemanticMapFromFilesAsync(SemanticOptions options, string filePath, CancellationToken cancellationToken)
    {
        var codeFiles = new[] { filePath };

        return CreateSemanticMapFromFilesAsync(options, codeFiles, cancellationToken);
    }
}