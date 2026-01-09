using ContextKit.Model;

namespace SemanticKit.Model;

public interface ISyntaxRouterBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
    ISemanticSyntaxRouter<TContext> CreateRouter();
}
