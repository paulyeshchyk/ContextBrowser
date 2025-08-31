using ContextKit.Model;
using RoslynKit.Phases.Syntax.Parsers;
using SemanticKit.Model.Options;

namespace ContextBrowser.Model;

public class InvocationFileParser : IFileParser
{
    private readonly RoslynInvocationParser<ContextInfo> _parser;

    public InvocationFileParser(RoslynInvocationParser<ContextInfo> parser)
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