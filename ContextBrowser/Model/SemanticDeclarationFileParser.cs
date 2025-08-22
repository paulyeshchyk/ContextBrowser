using ContextKit.Model;
using SemanticKit.Model;

namespace ContextBrowser.Model;

public class SemanticDeclarationFileParser : IFileParser
{
    private readonly SemanticDeclarationParser<ContextInfo> _parser;

    public SemanticDeclarationFileParser(SemanticDeclarationParser<ContextInfo> parser)
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