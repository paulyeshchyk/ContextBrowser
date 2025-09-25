namespace SemanticKit.Model;

public interface ISemanticInvocationResolver
{
    ISemanticModelWrapper? Resolve(object expressionSyntax);

    ISemanticModelWrapper? Resolve(ISyntaxTreeWrapper? syntaxTree);
}
