using SemanticKit.Model;

namespace SemanticKit.Parsers.Strategy.Invocation;

public interface IInvocationResolver<TSyntaxTreeWrapper>
    where TSyntaxTreeWrapper : ISyntaxTreeWrapper
{
    ISemanticModelWrapper? Resolve(TSyntaxTreeWrapper? syntaxTree);
}
