using System.Collections.Generic;
using System.Threading;
using ContextKit.Model;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace SemanticKit.Parsers.File;

// context: file, directory, contextInfo, read
public class SemanticDeclarationFileParser : IFileParser
{
    private readonly ISemanticDeclarationParser<ContextInfo> _parser;

    public SemanticDeclarationFileParser(ISemanticDeclarationParser<ContextInfo> parser)
    {
        _parser = parser;
    }

    // context: file, directory, contextInfo, read
    public IEnumerable<ContextInfo> ParseFiles(string[] filePaths, SemanticOptions options, CancellationToken ct)
    {
        return _parser.ParseFiles(filePaths, options, ct);
    }

    // context: file, directory, contextInfo, read
    public void RenewContextInfoList(IEnumerable<ContextInfo> contextInfoList)
    {
        _parser.RenewContextInfoList(contextInfoList);
    }
}