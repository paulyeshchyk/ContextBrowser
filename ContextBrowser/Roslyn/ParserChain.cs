using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowser.Model;
using ContextKit.Model;
using SemanticKit.Model.Options;

namespace ContextBrowser.Roslyn;

// context: roslyn, directory, contextInfo, build
public sealed class ParserChain : IContextParser<ContextInfo>
{
    private readonly SortedList<int, IFileParser> _parsers;

    public ParserChain(SortedList<int, IFileParser> parsers)
    {
        _parsers = parsers;
    }

    // context: roslyn, read, directory, contextInfo
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