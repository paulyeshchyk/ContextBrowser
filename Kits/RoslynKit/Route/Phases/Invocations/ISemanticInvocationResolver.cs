using Microsoft.CodeAnalysis.CSharp.Syntax;
using SemanticKit.Model;

namespace RoslynKit.Route.Phases.Invocations;

public interface ISemanticInvocationResolver
{
    ISemanticModelWrapper? Resolve(InvocationExpressionSyntax expressionSyntax);

    ISemanticModelWrapper? Resolve(ISyntaxTreeWrapper? syntaxTree);
}
