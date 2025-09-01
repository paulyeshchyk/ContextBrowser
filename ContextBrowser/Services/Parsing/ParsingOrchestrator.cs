using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowser.ContextCommentsParser;
using ContextBrowser.FileManager;
using ContextBrowser.Infrastructure;
using ContextBrowser.Model;
using ContextBrowser.Roslyn;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKitSourceAttribute.TraceGenerator;
using ContextKit.Model;
using LoggerKit;

namespace ContextBrowser.Services;

// context: parsing, build
public interface IParsingOrchestrator
{
    // context: parsing, build
    Task<IEnumerable<ContextInfo>> GetParsedContextsAsync(AppOptions options, CancellationToken cancellationToken);
}

// context: parsing, build
public class ParsingOrchestrator : IParsingOrchestrator
{
    private readonly IContextInfoCacheService _contextInfoCacheService;
    private readonly ICodeParseService _codeParseService;
    private readonly IDeclarationParserFactory _declarationParserFactory;
    private readonly IReferenceParserFactory _referenceParserFactory;
    private readonly IAppLogger<AppLevel> _logger;

    public ParsingOrchestrator(
        IContextInfoCacheService contextInfoCacheService,
        ICodeParseService codeParseService,
        IDeclarationParserFactory declarationParserFactory,
        IReferenceParserFactory referenceParserFactory,
        IAppLogger<AppLevel> logger)
    {
        _contextInfoCacheService = contextInfoCacheService;
        _codeParseService = codeParseService;
        _declarationParserFactory = declarationParserFactory;
        _referenceParserFactory = referenceParserFactory;
        _logger = logger;
    }

    // context: parsing, build
    public async Task<IEnumerable<ContextInfo>> GetParsedContextsAsync(AppOptions options, CancellationToken cancellationToken)
    {
        var cacheModel = options.Export.Paths.CacheModel;

        Test(_logger.WriteLog);


        // Try to read from cache first
        var contextsFromCache = await _contextInfoCacheService.ReadContextsFromCache(
            cacheModel,
            (token) => Task.FromResult<IEnumerable<ContextInfo>>(Enumerable.Empty<ContextInfo>()), // Placeholder to satisfy signature
            cancellationToken);

        if (contextsFromCache.Any())
        {
            return contextsFromCache;
        }

        var declarationParser = _declarationParserFactory.Create(options.ParsingOptions.SemanticOptions);
        var referenceParser = _referenceParserFactory.Create();

        var declarationFileParser = new SemanticDeclarationFileParser(declarationParser);
        var referenceFileParser = new ReferenceFileParser(referenceParser);

        var fileParsers = new SortedList<int, IFileParser> {
            { 0, declarationFileParser },
            { 1, referenceFileParser }
        };

        var parserChain = new ParserChain(fileParsers);

        var result = contextsFromCache = await _codeParseService.Parse(parserChain, cancellationToken);

        // Save to cache in a fire-and-forget task
        _ = Task.Run(async () =>
        {
            await _contextInfoCacheService.SaveContextsToCacheAsync(cacheModel, result, cancellationToken).ConfigureAwait(false);
        }, cancellationToken);

        if (!result.Any())
        {
            _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Err, "Context list is empty");
            return Enumerable.Empty<ContextInfo>();
        }
        return result;
    }


    [TraceMethodStart("onWriteLog", (int)AppLevel.R_Dll, (int)LogLevel.Cntx, "Compilation map building for: z")]
    internal static void Test(OnWriteLog? onWriteLog)
    {
        onWriteLog?.Invoke(AppLevel.App, LogLevel.Cntx, "test2")
    }
}