namespace SemanticKit.Model;

public interface IInvocationNodeWrapper
{
    ISemanticModelWrapper? GetSemanticModel();

    ISyntaxTreeWrapper GetTree();

    object Expression { get; }
}