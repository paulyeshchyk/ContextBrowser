using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowser.FileManager;
using ContextBrowser.Infrastructure;
using ContextBrowser.Model;
using ContextBrowser.Roslyn;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
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
    private readonly IContextInfoRelationManager _relationManager;

    public ParsingOrchestrator(
        IContextInfoCacheService contextInfoCacheService,
        ICodeParseService codeParseService,
        IDeclarationParserFactory declarationParserFactory,
        IReferenceParserFactory referenceParserFactory,
        IContextInfoRelationManager relationManager,
        IAppLogger<AppLevel> logger)
    {
        _contextInfoCacheService = contextInfoCacheService;
        _codeParseService = codeParseService;
        _declarationParserFactory = declarationParserFactory;
        _referenceParserFactory = referenceParserFactory;
        _relationManager = relationManager;
        _logger = logger;
    }

    // context: parsing, build
    public async Task<IEnumerable<ContextInfo>> GetParsedContextsAsync(AppOptions options, CancellationToken cancellationToken)
    {
        var cacheModel = options.Export.FilePaths.CacheModel;

        var result = await _contextInfoCacheService.GetOrParseAndCacheAsync(
            cacheModel,
            (token) => ParsingJobAsync(options, token),
            _relationManager.ConvertToContextInfoAsync,
            cancellationToken);

        if (!result.Any())
        {
            _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Err, "Context list is empty");
            return Enumerable.Empty<ContextInfo>();
        }
        return result;
    }

    private async Task<IEnumerable<ContextInfo>> ParsingJobAsync(AppOptions options, CancellationToken token)
    {
        var declarationParser = _declarationParserFactory.Create(options.ParsingOptions.SemanticOptions);
        var referenceParser = _referenceParserFactory.Create();

        var declarationFileParser = new SemanticDeclarationFileParser(declarationParser);
        var referenceFileParser = new ReferenceFileParser(referenceParser);

        var fileParsers = new SortedList<int, IFileParser> {
                { 0, declarationFileParser },
                { 1, referenceFileParser }
            };

        var parserChain = new ParserChain(fileParsers);
        return await _codeParseService.Parse(parserChain, token);
    }
}