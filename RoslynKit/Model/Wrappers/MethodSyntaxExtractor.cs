using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynKit.Model.Wrappers;

public static class MethodSyntaxExtractor
{
    public static MethodSyntaxWrapper? Extract(MethodDeclarationSyntax? resultSyntax, SemanticModel model)
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

        return new MethodSyntaxWrapper(symbol: symbol, syntax: resultSyntax);
    }
}
