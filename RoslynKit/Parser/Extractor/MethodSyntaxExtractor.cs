using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;

namespace RoslynKit.Parser.Extractor;

public static class MethodSyntaxExtractor
{
    public static MethodSyntaxModel? Extract(MethodDeclarationSyntax? resultSyntax, SemanticModel model)
    {
        if(resultSyntax == null)
        {
            return default;
        }

        var methodName = resultSyntax.Identifier.Text;
        var symbol = model.GetDeclaredSymbol(resultSyntax);
        if(symbol == null)
        {
            Console.WriteLine($"[ERR] Symbol not found for {methodName}");
            return default;
        }

        var fullMemberName = symbol.GetFullMemberName();
        return new MethodSyntaxModel() { methodFullName = fullMemberName, methodName = methodName, spanStart = resultSyntax.Span.Start, spanEnd = resultSyntax.Span.End, Symbol = symbol };
    }
}


public record MethodSyntaxModel
{
    public string? methodName;
    public string? methodFullName;
    public int spanStart;
    public int spanEnd;
    public ISymbol? Symbol;
}