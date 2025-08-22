using ContextKit.Model;
using RoslynKit.Phases.Syntax.Parsers;

namespace ContextBrowser.Model;

public class InvocationFileParser : IFileParser
{
    private readonly RoslynInvocationParser<ContextInfo> _parser;

    public InvocationFileParser(RoslynInvocationParser<ContextInfo> parser)
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