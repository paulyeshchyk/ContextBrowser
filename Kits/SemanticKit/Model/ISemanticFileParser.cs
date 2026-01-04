using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

public interface ISemanticFileParser<TContext>
{
    Task<IEnumerable<TContext>> ParseFilesAsync(IEnumerable<string> codeFiles, SemanticOptions options, CancellationToken cancellationToken);

    void RenewContextInfoList(IEnumerable<TContext> contextInfoList);
}
