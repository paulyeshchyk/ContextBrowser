using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Import;
using ContextKit.Model;
using LoggerKit;
using SemanticKit.Model.Options;
using SemanticKit.Parsers.File;

namespace ContextBrowser.Services.Parsing;

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

    // context: parsing, build, compilationFlow
    public Task<IEnumerable<ContextInfo>> ParseAsync(IFileParserPipeline<ContextInfo> pipeline, CancellationToken cancellationToken)
    {
        var importOptions = _optionsStore.GetOptions<ImportOptions>();
        var parsingOptions = _optionsStore.GetOptions<CodeParsingOptions>();

        var filePaths = PathAnalyzer.GetFilePaths(importOptions.SearchPaths, importOptions.FileExtensions, _logger.WriteLog);
        var filtered = PathFilter.FilteroutPaths(filePaths, importOptions.Exclude, (thePath) => thePath);

        if (filtered.Length == 0)
        {
            throw new Exception("No files to parse");
        }
        return pipeline.ParseAsync(filtered, parsingOptions.SemanticOptions, cancellationToken);
    }
}
