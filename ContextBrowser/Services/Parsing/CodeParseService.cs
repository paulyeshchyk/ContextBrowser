using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowser.Infrastructure;
using ContextBrowser.Model;
using ContextBrowser.Roslyn;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Import;
using ContextKit.Model;
using LoggerKit;
using SemanticKit.Model.Options;

namespace ContextBrowser.Services;

// context: parsing, build
public interface ICodeParseService
{
    // context: parsing, build
    Task<IEnumerable<ContextInfo>> Parse(IContextParser<ContextInfo> contextParser, CancellationToken cancellationToken);
}

// context: parsing, build
public class CodeParseService : ICodeParseService
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IAppOptionsStore _optionsStore;

    public CodeParseService(IAppLogger<AppLevel> logger, IAppOptionsStore optionsStore)
    {
        _logger = logger;
        _optionsStore = optionsStore;
    }

    // context: parsing, build
    public Task<IEnumerable<ContextInfo>> Parse(IContextParser<ContextInfo> contextParser, CancellationToken cancellationToken)
    {
        var importOptions = _optionsStore.GetOptions<ImportOptions>();
        var parsingOptions = _optionsStore.GetOptions<CodeParsingOptions>();

        var filePaths = PathAnalyzer.GetFilePaths(importOptions.SearchPaths, importOptions.FileExtensions, _logger.WriteLog);
        var filtered = PathFilter.FilteroutPaths(filePaths, importOptions.Exclude, (thePath) => thePath);

        if (!filtered.Any())
        {
            throw new Exception("No files to parse");
        }
        return contextParser.ParseAsync(filtered, parsingOptions.SemanticOptions, cancellationToken);
    }
}
