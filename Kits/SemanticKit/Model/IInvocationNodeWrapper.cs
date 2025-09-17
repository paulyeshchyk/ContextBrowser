namespace SemanticKit.Model;

public interface IInvocationNodeWrapper
{
    ISemanticModelWrapper? GetSemanticModel();

    ISyntaxTreeWrapper BuildTree();

    object Expression { get; }
}