using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynKit.Model.Wrappers;

public static class TypeSyntaxExtractor
{
    public static TypeSyntaxWrapper? Extract(TypeDeclarationSyntax? resultSyntax, SemanticModel model)
    {
        if(resultSyntax == null)
        {
            return default;
        }

        ISymbol? symbol = model.GetDeclaredSymbol(resultSyntax);

        if(symbol == null)
        {
            return default;
        }

        return new TypeSyntaxWrapper(symbol: symbol, syntax: resultSyntax);
    }
}
