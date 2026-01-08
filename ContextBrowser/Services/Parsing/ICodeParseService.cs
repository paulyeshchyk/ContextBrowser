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
public interface ICodeParseService
{
    // context: parsing, build
    Task<IEnumerable<ContextInfo>> ParseAsync(IFileParserPipeline<ContextInfo> pipeline, CancellationToken cancellationToken);
}
