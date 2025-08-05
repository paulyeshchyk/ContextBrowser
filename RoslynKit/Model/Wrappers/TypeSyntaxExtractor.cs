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
        var symbol = model.GetDeclaredSymbol(resultSyntax);
        if(symbol == null)
        {
            Console.WriteLine($"[ERR] Symbol not found for {resultSyntax.Identifier.Text}");
            return default;
        }

        return new TypeSyntaxWrapper(symbol: symbol, syntax: resultSyntax);
    }
}
