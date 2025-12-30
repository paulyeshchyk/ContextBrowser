using ContextKit.Model;

namespace SemanticKit.Model;

public interface ISemanticSyntaxRouterBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
    ISemanticSyntaxRouter<TContext> CreateRouter();
}
