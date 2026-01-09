using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.CacheManager;
using LoggerKit;
using SemanticKit.Model;
using SemanticKit.Parsers.File;
using SemanticKit.Parsers.Strategy.Declaration;
using SemanticKit.Parsers.Strategy.Invocation;

namespace SemanticKit.Parsers.Strategy;

// context: parsing, build
public interface IParsingOrchestrator
{
    // context: parsing, build, compilationFlow
    Task<IEnumerable<ContextInfo>> ParseAsync(CancellationToken cancellationToken);
}

// context: parsing, build
public class ParsingOrchestrator<TSyntaxTreeWrapper> : IParsingOrchestrator
    where TSyntaxTreeWrapper : ISyntaxTreeWrapper
{
    private readonly IContextInfoCacheService _contextInfoCacheService;
    private readonly ICodeParseService _codeParseService;
    private readonly IReferenceParserFactory<TSyntaxTreeWrapper> _invocationParserFactory;
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoRelationManager _relationManager;
    private readonly IDeclarationFileParser<ContextInfo> _declarationParser;

    public ParsingOrchestrator(
        IContextInfoCacheService contextInfoCacheService,
        ICodeParseService codeParseService,
        IReferenceParserFactory<TSyntaxTreeWrapper> referenceParserFactory,
        IContextInfoRelationManager relationManager,
        IAppLogger<AppLevel> logger,
        IDeclarationFileParser<ContextInfo> declarationParser)
    {
        _contextInfoCacheService = contextInfoCacheService;
        _codeParseService = codeParseService;
        _invocationParserFactory = referenceParserFactory;
        _relationManager = relationManager;
        _logger = logger;
        _declarationParser = declarationParser;
    }

    // context: parsing, build, compilationFlow
    public async Task<IEnumerable<ContextInfo>> ParseAsync(CancellationToken cancellationToken)
    {
        var result = await _contextInfoCacheService.GetOrParseAndCacheAsync
        (
                      parseJob: ParsingJobAsync,
            onRelationCallback: _relationManager.ConvertToContextInfoAsync,
             cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        if (!result.Any())
        {
            _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Err, "Context list is empty");
            return [];
        }
        return result;
    }

    // context: parsing, build, compilationFlow
    internal Task<IEnumerable<ContextInfo>> ParsingJobAsync(CancellationToken token)
    {
        _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Cntx, "Starting parsing job");

        var referenceParser = _invocationParserFactory.Create();

        var declarationFileParserWrapper = new DeclarationFileParserWrapper(_declarationParser);
        var invocationFileParserWrapper = new InvocationFileParserWrapper(referenceParser);

        var fileParsers = new SortedList<int, IFileParser<ContextInfo>> {
                { 0, declarationFileParserWrapper },
                { 1, invocationFileParserWrapper }
            };

        var pipeline = new FileParserPipeline(fileParsers);
        return _codeParseService.ParseAsync(pipeline, token);
    }
}