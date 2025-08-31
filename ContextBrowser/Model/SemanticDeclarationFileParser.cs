using ContextKit.Model;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace ContextBrowser.Model;

public class SemanticDeclarationFileParser : IFileParser
{
    private readonly ISemanticDeclarationParser<ContextInfo> _parser;

    public SemanticDeclarationFileParser(ISemanticDeclarationParser<ContextInfo> parser)
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