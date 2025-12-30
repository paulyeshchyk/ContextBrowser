using ContextKit.Model;

namespace SemanticKit.Model;

public interface ISemanticSyntaxRouterBuilderRegistry<TContext>
    where TContext : IContextWithReferences<TContext>
{
    ISemanticSyntaxRouterBuilder<TContext> GetRouterBuilder(string technology);
}