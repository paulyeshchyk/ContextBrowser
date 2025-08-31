using SemanticKit.Model.Options;

namespace SemanticKit.Model;

public interface ISemanticDeclarationParser<TContext>
{
    IEnumerable<TContext> ParseFiles(IEnumerable<string> codeFiles, SemanticOptions options, CancellationToken cancellationToken);

    void RenewContextInfoList(IEnumerable<TContext> contextInfoList);
}
