using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Syntax.Parser.Wrappers;

namespace RoslynKit.Syntax.Parser.Extractor;

public static class CSharpTypeSyntaxExtractor
{
    public static CSharpTypeSyntaxWrapper? Extract(TypeDeclarationSyntax? resultSyntax, SemanticModel model)
    {
        if (resultSyntax == null)
        {
            return default;
        }

        ISymbol? symbol = model.GetDeclaredSymbol(resultSyntax);

        if (symbol == null)
        {
            return default;
        }

        return new CSharpTypeSyntaxWrapper(symbol: symbol, syntax: resultSyntax);
    }
}
