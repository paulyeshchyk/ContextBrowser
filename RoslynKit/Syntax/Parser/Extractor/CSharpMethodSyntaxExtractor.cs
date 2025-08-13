using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Syntax.Parser.Wrappers;

namespace RoslynKit.Syntax.Parser.Extractor;

public static class CSharpMethodSyntaxExtractor
{
    public static CSharpMethodSyntaxWrapper? Extract(MethodDeclarationSyntax? resultSyntax, SemanticModel model)
    {
        if (resultSyntax == null)
        {
            return default;
        }

        var symbol = model.GetDeclaredSymbol(resultSyntax);
        if (symbol == null)
        {
            Console.WriteLine($"[ERR] Symbol not found for {resultSyntax.Identifier.Text}");
            return default;
        }

        return new CSharpMethodSyntaxWrapper(symbol: symbol, syntax: resultSyntax);
    }
}
