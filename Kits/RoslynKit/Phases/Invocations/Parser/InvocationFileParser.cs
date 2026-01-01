using System;
using System.Collections.Generic;
using System.Threading;
using ContextKit.Model;
using SemanticKit.Model.Options;
using SemanticKit.Parsers.File;

namespace RoslynKit.Phases.Invocations.Parser;

[Obsolete]
public class InvocationFileParser : IFileParser<ContextInfo>
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