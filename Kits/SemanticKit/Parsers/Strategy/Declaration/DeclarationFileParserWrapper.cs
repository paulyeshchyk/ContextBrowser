using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;
using SemanticKit.Model;
using SemanticKit.Model.Options;
using SemanticKit.Parsers.File;

namespace SemanticKit.Parsers.Strategy.Declaration;

// context: file, directory, ContextInfo, read
public class DeclarationFileParserWrapper : IFileParser<ContextInfo>
{
    private readonly IDeclarationFileParser<ContextInfo> _parser;

    public DeclarationFileParserWrapper(IDeclarationFileParser<ContextInfo> parser)
    {
        _parser = parser;
    }

    // context: file, directory, ContextInfo, read, compilationFlow
    public Task<IEnumerable<ContextInfo>> ParseFilesAsync(string[] filePaths, string compilationName, CancellationToken ct)
    {
        return _parser.ParseFilesAsync(filePaths, compilationName, ct);
    }

    // context: file, directory, ContextInfo, read
    public void RenewContextInfoList(IEnumerable<ContextInfo> contextInfoList)
    {
        _parser.RenewContextInfoList(contextInfoList);
    }
}