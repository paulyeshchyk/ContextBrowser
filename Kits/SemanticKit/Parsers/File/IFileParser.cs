using System.Collections.Generic;
using System.Threading;
using ContextKit.Model;
using SemanticKit.Model.Options;

namespace SemanticKit.Parsers.File;

public interface IFileParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    IEnumerable<TContext> ParseFiles(string[] filePaths, SemanticOptions options, CancellationToken ct);

    void RenewContextInfoList(IEnumerable<TContext> contextInfoList);
}
