using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace SemanticKit.Parsers.File;

// context: file, directory, ContextInfo, read
public class ReferenceFileParser : IFileParser<ContextInfo>
{
    private readonly IInvocationParser<ContextInfo> _parser;

    public ReferenceFileParser(IInvocationParser<ContextInfo> parser)
    {
        _parser = parser;
    }

    // context: file, directory, syntax, ContextInfo, read
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