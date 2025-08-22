using ContextBrowser.Model;
using ContextKit.Model;

namespace ContextBrowser.Roslyn;

// context: roslyn, directory, contextInfo, build
public sealed class RoslynContextParser : IContextParser<ContextInfo>
{
    private readonly SortedList<int, IFileParser> _parsers;

    public RoslynContextParser(SortedList<int, IFileParser> parsers)
    {
        _parsers = parsers;
    }

    // context: roslyn, read, directory, contextInfo
    public Task<IEnumerable<ContextInfo>> ParseAsync(string[] filePaths, CancellationToken cancellationToken)
    {
        var result = new List<ContextInfo>();
        foreach (var parser in _parsers.OrderBy(p => p.Key).Select(p => p.Value))
        {
            parser.RenewContextInfoList(result);
            result = parser.ParseFiles(filePaths, cancellationToken).ToList();
        }
        return Task.FromResult((IEnumerable<ContextInfo>)result);
    }
}
