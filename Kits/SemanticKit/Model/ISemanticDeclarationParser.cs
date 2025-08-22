namespace SemanticKit.Model;

public interface ISemanticDeclarationParser<TContext>
{
    IEnumerable<TContext> ParseFiles(IEnumerable<string> codeFiles, CancellationToken cancellationToken);

    void RenewContextInfoList(IEnumerable<TContext> contextInfoList);
}
