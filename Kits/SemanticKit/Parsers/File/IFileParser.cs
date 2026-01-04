using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;
using SemanticKit.Model.Options;

namespace SemanticKit.Parsers.File;

public interface IFileParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    Task<IEnumerable<TContext>> ParseFilesAsync(string[] filePaths, SemanticOptions options, CancellationToken ct);

    void RenewContextInfoList(IEnumerable<TContext> contextInfoList);
}
