using ContextKit.Model;
using SemanticKit.Model;

namespace ContextBrowser.Model;

public class ReferenceFileParser : IFileParser
{
    private readonly IInvocationParser _parser;

    public ReferenceFileParser(IInvocationParser parser)
    {
        _parser = parser;
    }

    public IEnumerable<ContextInfo> ParseFiles(string[] filePaths, CancellationToken ct)
    {
        return _parser.ParseFiles(filePaths, ct);
    }

    public void RenewContextInfoList(IEnumerable<ContextInfo> contextInfoList)
    {
        _parser.RenewContextInfoList(contextInfoList);
    }
}