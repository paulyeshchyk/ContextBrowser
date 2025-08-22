namespace SemanticKit.Model;

public interface ISemanticSyntaxRouter<TContext>
{
    void Route(IEnumerable<object> availableSyntaxies, ISemanticModelWrapper model, CancellationToken cancellationToken);
}
