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

namespace ContextBrowser.Services.Parsing;

// context: parsing, build
public interface IParsingOrchestrator
{
    // context: parsing, build
    Task<IEnumerable<ContextInfo>> ParseAsync(CancellationToken cancellationToken);
}

// context: parsing, build
public class ParsingOrchestrator : IParsingOrchestrator
{
    private readonly IContextInfoCacheService _contextInfoCacheService;
    private readonly ICodeParseService _codeParseService;
    private readonly IReferenceParserFactory _referenceParserFactory;
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoRelationManager _relationManager;
    private readonly ISemanticDeclarationParser<ContextInfo> _declarationParser;

    public ParsingOrchestrator(
        IContextInfoCacheService contextInfoCacheService,
        ICodeParseService codeParseService,
        IReferenceParserFactory referenceParserFactory,
        IContextInfoRelationManager relationManager,
        IAppLogger<AppLevel> logger,
        ISemanticDeclarationParser<ContextInfo> declarationParser)
    {
        _contextInfoCacheService = contextInfoCacheService;
        _codeParseService = codeParseService;
        _referenceParserFactory = referenceParserFactory;
        _relationManager = relationManager;
        _logger = logger;
        _declarationParser = declarationParser;
    }

    // context: parsing, build
    public async Task<IEnumerable<ContextInfo>> ParseAsync(CancellationToken cancellationToken)
    {
        var result = await _contextInfoCacheService.GetOrParseAndCacheAsync(
            ParsingJobAsync,
            _relationManager.ConvertToContextInfoAsync,
            cancellationToken).ConfigureAwait(false);

        var orchestratedContextsAsync = result.ToList();
        if (orchestratedContextsAsync.Count != 0)
        {
            return orchestratedContextsAsync;
        }

        _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Err, "Context list is empty");
        return [];
    }

    // context: parsing, build
    internal Task<IEnumerable<ContextInfo>> ParsingJobAsync(CancellationToken token)
    {
        var referenceParser = _referenceParserFactory.Create();

        var declarationFileParser = new SemanticDeclarationFileParser(_declarationParser);
        var referenceFileParser = new ReferenceFileParser(referenceParser);

        var fileParsers = new SortedList<int, IFileParser<ContextInfo>> {
                { 0, declarationFileParser },
                { 1, referenceFileParser }
            };

        var parserChain = new FileParserPipeline(fileParsers);
        return _codeParseService.ParseAsync(parserChain, token);
    }
}
