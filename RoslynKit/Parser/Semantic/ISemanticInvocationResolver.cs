using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynKit.Parser.Semantic;

public interface ISemanticInvocationResolver
{
    SemanticModel? Resolve(InvocationExpressionSyntax expressionSyntax);

    SemanticModel? Resolve(SyntaxTree? syntaxTree);
}