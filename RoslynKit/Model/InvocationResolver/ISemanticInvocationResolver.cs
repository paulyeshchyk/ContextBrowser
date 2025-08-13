using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynKit.Model.Resolver;

public interface ISemanticInvocationResolver
{
    SemanticModel? Resolve(InvocationExpressionSyntax expressionSyntax);

    SemanticModel? Resolve(SyntaxTree? syntaxTree);
}