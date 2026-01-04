using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

public interface IInvocationParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    void ParseCode(string code, string filePath, SemanticOptions options, CancellationToken cancellationToken);

    void ParseFile(string filePath, SemanticOptions options, CancellationToken cancellationToken);

    Task<IEnumerable<TContext>> ParseFilesAsync(string[] filePaths, SemanticOptions options, CancellationToken cancellationToken);

    void RenewContextInfoList(IEnumerable<TContext> contextInfoList);
}
