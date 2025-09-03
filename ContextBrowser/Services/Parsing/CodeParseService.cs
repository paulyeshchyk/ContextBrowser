using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowser.Infrastructure;
using ContextBrowser.Model;
using ContextBrowser.Roslyn;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKitSourceAttribute.TraceGenerator;
using ContextKit.Model;
using LoggerKit;

namespace ContextBrowser.Services;

public interface ICodeParseService
{
    Task<IEnumerable<ContextInfo>> Parse(IContextParser<ContextInfo> contextParser, CancellationToken cancellationToken);
}

public class CodeParseService : ICodeParseService
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IAppOptionsStore _optionsStore;

    public CodeParseService(IAppLogger<AppLevel> logger, IAppOptionsStore optionsStore)
    {
        _logger = logger;
        _optionsStore = optionsStore;
    }

    // context: app
    public Task<IEnumerable<ContextInfo>> Parse(IContextParser<ContextInfo> contextParser, CancellationToken cancellationToken)
    {
        Test(_logger.WriteLog);
        var importOptions = _optionsStore.Options().Import;
        var parsingOptions = _optionsStore.Options().ParsingOptions;

        var filePaths = PathAnalyzer.GetFilePaths(importOptions.SearchPaths, importOptions.FileExtensions, _logger.WriteLog);
        var filtered = PathFilter.FilteroutPaths(filePaths, importOptions.Exclude, (thePath) => thePath);

        if (!filtered.Any())
        {
            throw new Exception("No files to parse");
        }
        return contextParser.ParseAsync(filtered, parsingOptions.SemanticOptions, cancellationToken);
    }

    [TraceMethodStart("onWriteLog", (int)AppLevel.App, (int)LogLevel.Cntx, "Text111")]
    internal static void Test(OnWriteLog? onWriteLog)
    {

    }
}
