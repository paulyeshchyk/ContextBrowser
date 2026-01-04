namespace SemanticKit.Model;

public interface IInvocationNodeWrapper<TSyntaxTreeWrapper>
    where TSyntaxTreeWrapper : ISyntaxTreeWrapper
{
    ISemanticModelWrapper? GetSemanticModel();

    TSyntaxTreeWrapper BuildTree();

    object Expression { get; }
}