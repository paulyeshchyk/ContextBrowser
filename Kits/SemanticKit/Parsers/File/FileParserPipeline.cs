using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;
using SemanticKit.Model.Options;

namespace SemanticKit.Parsers.File;

// context: file, directory, contextInfo, read
public sealed class FileParserPipeline : IFileParserPipeline<ContextInfo>
{
    private readonly SortedList<int, IFileParser> _parsers;

    public FileParserPipeline(SortedList<int, IFileParser> parsers)
    {
        _parsers = parsers;
    }

    // context: file, directory, contextInfo, read
    public Task<IEnumerable<ContextInfo>> ParseAsync(string[] filePaths, SemanticOptions options, CancellationToken cancellationToken)
    {
        var result = new List<ContextInfo>();
        foreach (var parser in _parsers.OrderBy(p => p.Key).Select(p => p.Value))
        {
            parser.RenewContextInfoList(result);
            result = parser.ParseFiles(filePaths, options, cancellationToken).ToList();
        }
        return Task.FromResult((IEnumerable<ContextInfo>)result);
    }
}