using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SemanticKit.Model;

namespace RoslynKit.Wrappers.Meta;

public class RoslynSemanticModelWrapper : ISemanticModelWrapper
{
    private readonly SemanticModel _semanticModel;

    public RoslynSemanticModelWrapper(SemanticModel semanticModel)
    {
        _semanticModel = semanticModel;
    }

    public object? GetSymbolInfo(object node, CancellationToken cancellationToken)
    {
        if (node is SyntaxNode syntaxNode)
        {
            var symbolInfo = _semanticModel.GetSymbolInfo(syntaxNode, cancellationToken);
            return symbolInfo;
        }
        return null;
    }

    public object? GetDeclaredSymbol(object syntax, CancellationToken cancellationToken)
    {
        if (syntax is MemberDeclarationSyntax syntaxNode)
        {
            var declaredSymbol = _semanticModel.GetDeclaredSymbol(syntaxNode, cancellationToken);
            return declaredSymbol;
        }
        throw new NotImplementedException("unknown syntax");
    }

    public object? GetSymbolForInvocation(object invocationNode)
    {
        if (invocationNode is SyntaxNode syntaxNode)
        {
            var symbolInfo = _semanticModel.GetSymbolInfo(syntaxNode);
            return symbolInfo.Symbol;
        }
        return null;
    }

    public object? GetTypeInfo(object syntax)
    {
        if (syntax is TypeSyntax syntaxNode)
        {
            return _semanticModel.GetTypeInfo(syntaxNode).Type;
        }
        throw new Exception("syntax is not MemberDeclarationSyntax");
    }
}