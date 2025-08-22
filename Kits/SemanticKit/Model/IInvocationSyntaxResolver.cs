namespace SemanticKit.Model;

public interface IInvocationSyntaxResolver
{
    IInvocationSyntaxWrapper? ResolveInvocationSymbol(object invocation, CancellationToken cancellationToken);
}
