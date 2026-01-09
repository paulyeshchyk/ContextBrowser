using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SemanticKit.Model.Options;

namespace SemanticKit.Parsers.File;

public interface IDeclarationFileParser<TContext>
{
    Task<IEnumerable<TContext>> ParseFilesAsync(IEnumerable<string> codeFiles, string compilationName, CancellationToken cancellationToken);

    void RenewContextInfoList(IEnumerable<TContext> contextInfoList);
}
