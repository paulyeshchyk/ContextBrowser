using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;

namespace SemanticKit.Parsers.File;

public interface IFileParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    Task<IEnumerable<TContext>> ParseFilesAsync(string[] filePaths, string compilationName, CancellationToken ct);

    void RenewContextInfoList(IEnumerable<TContext> contextInfoList);
}
