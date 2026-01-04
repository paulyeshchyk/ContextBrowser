using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace SemanticKit.Parsers.File;

// context: file, directory, ContextInfo, read
public class SemanticDeclarationFileParser : IFileParser<ContextInfo>
{
    private readonly ISemanticFileParser<ContextInfo> _parser;

    public SemanticDeclarationFileParser(ISemanticFileParser<ContextInfo> parser)
    {
        _parser = parser;
    }

    // context: file, directory, ContextInfo, read
    public Task<IEnumerable<ContextInfo>> ParseFilesAsync(string[] filePaths, SemanticOptions options, CancellationToken ct)
    {
        return _parser.ParseFilesAsync(filePaths, options, ct);
    }

    // context: file, directory, ContextInfo, read
    public void RenewContextInfoList(IEnumerable<ContextInfo> contextInfoList)
    {
        _parser.RenewContextInfoList(contextInfoList);
    }
}