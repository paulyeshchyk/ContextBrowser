using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;
using SemanticKit.Model.Options;
using SemanticKit.Parsers.File;

namespace SemanticKit.Parsers.Strategy.Invocation;

// context: file, directory, ContextInfo, read
public class InvocationFileParserWrapper : IFileParser<ContextInfo>
{
    private readonly IInvocationFileParser<ContextInfo> _parser;

    public InvocationFileParserWrapper(IInvocationFileParser<ContextInfo> parser)
    {
        _parser = parser;
    }

    // context: file, directory, syntax, ContextInfo, read
    public Task<IEnumerable<ContextInfo>> ParseFilesAsync(string[] filePaths, string compilationName, CancellationToken ct)
    {
        return _parser.ParseFilesAsync(filePaths, ct);
    }

    // context: file, directory, ContextInfo, read
    public void RenewContextInfoList(IEnumerable<ContextInfo> contextInfoList)
    {
        _parser.RenewContextInfoList(contextInfoList);
    }
}