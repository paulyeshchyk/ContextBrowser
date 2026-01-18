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
    private readonly IAppOptionsStore _optionsStore;

    public RoslynCompilationMapBuilder(
        IAppLogger<AppLevel> logger,
        ISyntaxTreeParser<RoslynSyntaxTreeWrapper> syntaxTreeParser,
        ICompilationMapMapper<RoslynSyntaxTreeWrapper> compilationMapMapper,
        ICompilationBuilder<RoslynSyntaxTreeWrapper> compilationMapBuilder,
        IAppOptionsStore optionsStore)
    {
        _logger = logger;
        _syntaxTreeParser = syntaxTreeParser;
        _compilationMapMapper = compilationMapMapper;
        _compilationMapBuilder = compilationMapBuilder;
        _optionsStore = optionsStore;
    }

    // context: roslyn, build, compilationFlow
    public async Task<SemanticCompilationMap<RoslynSyntaxTreeWrapper>> CreateSemanticMapFromFilesAsync(IEnumerable<string> codeFiles, string compilationName, CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Cntx, $"Building compilation map for {codeFiles.Count()} files", LogLevelNode.None);

        var syntaxTrees = await _syntaxTreeParser.ParseFilesToSyntaxTreesAsync(codeFiles, cancellationToken).ConfigureAwait(false);

        var compilation = await _compilationMapBuilder.BuildAsync(syntaxTrees, compilationName, cancellationToken).ConfigureAwait(false);

        var result = _compilationMapMapper.MapSemanticModelToCompilationMap(syntaxTrees, compilation);

        return result;
    }

    // context: roslyn, build
    public Task<SemanticCompilationMap<RoslynSyntaxTreeWrapper>> CreateSemanticMapFromFilesAsync(string filePath, string compilationName, CancellationToken cancellationToken)
    {
        var codeFiles = new[] { filePath };

        return CreateSemanticMapFromFilesAsync(codeFiles, compilationName, cancellationToken);
    }
}