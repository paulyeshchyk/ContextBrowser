namespace SemanticKit.Model;

public interface ISemanticInvocationResolver<TSyntaxTreeWrapper>
    where TSyntaxTreeWrapper : ISyntaxTreeWrapper
{
    ISemanticModelWrapper? Resolve(TSyntaxTreeWrapper? syntaxTree);
}
