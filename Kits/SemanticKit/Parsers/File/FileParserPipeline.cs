using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;
using SemanticKit.Model.Options;

namespace SemanticKit.Parsers.File;

// context: file, directory, ContextInfo, read
public sealed class FileParserPipeline : IFileParserPipeline<ContextInfo>
{
    private readonly SortedList<int, IFileParser<ContextInfo>> _parsers;

    public FileParserPipeline(SortedList<int, IFileParser<ContextInfo>> parsers)
    {
        _parsers = parsers;
    }

    // context: file, directory, ContextInfo, read
    public async Task<IEnumerable<ContextInfo>> ParseAsync(string[] filePaths, SemanticOptions options, CancellationToken cancellationToken)
    {
        var result = Enumerable.Empty<ContextInfo>();

        foreach (var parser in _parsers.OrderBy(p => p.Key).Select(p => p.Value))
        {
            parser.RenewContextInfoList(result.ToList());

            result = await parser.ParseFilesAsync(filePaths, options, cancellationToken).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();
        }

        return result;
    }
}