namespace SemanticKit.Model;

public interface IInvocationSyntaxResolver
{
    BaseSyntaxWrapper? ResolveInvocationSymbol(object invocation, CancellationToken cancellationToken);
}
