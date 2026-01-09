using ContextKit.Model;
using SemanticKit.Model;

namespace SemanticKit.Parsers.Syntax;

public interface ISyntaxRouterBuilderRegistry<TContext>
    where TContext : IContextWithReferences<TContext>
{
    ISyntaxRouterBuilder<TContext> GetRouterBuilder(string technology);
}