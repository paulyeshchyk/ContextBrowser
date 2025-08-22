using SemanticKit.Model;

namespace RoslynKit.Phases.Invocations;

public interface ISemanticInvocationResolver
{
    ISemanticModelWrapper? Resolve(object expressionSyntax);

    ISemanticModelWrapper? Resolve(ISyntaxTreeWrapper? syntaxTree);
}
