using ContextKit.Model;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace ContextBrowser.Model;

public class ReferenceFileParser : IFileParser
{
    private readonly IInvocationParser _parser;

    public ReferenceFileParser(IInvocationParser parser)
    {
        _parser = parser;
    }

    public IEnumerable<ContextInfo> ParseFiles(string[] filePaths, SemanticOptions options, CancellationToken ct)
    {
        return _parser.ParseFiles(filePaths, options, ct);
    }

    public void RenewContextInfoList(IEnumerable<ContextInfo> contextInfoList)
    {
        _parser.RenewContextInfoList(contextInfoList);
    }
}