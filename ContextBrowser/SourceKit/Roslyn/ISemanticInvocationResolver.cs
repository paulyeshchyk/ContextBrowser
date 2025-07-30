using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ContextBrowser.SourceKit.Roslyn;

public interface ISemanticInvocationResolver
{
    SemanticModel? Resolve(InvocationExpressionSyntax expressionSyntax);

    SemanticModel? Resolve(SyntaxTree? syntaxTree);
}
