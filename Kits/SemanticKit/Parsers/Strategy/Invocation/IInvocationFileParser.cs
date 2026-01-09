using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;
using SemanticKit.Model.Options;

namespace SemanticKit.Parsers.Strategy.Invocation;

public interface IInvocationFileParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    Task<IEnumerable<TContext>> ParseFilesAsync(string[] filePaths, CancellationToken cancellationToken);

    void RenewContextInfoList(IEnumerable<TContext> contextInfoList);
}
