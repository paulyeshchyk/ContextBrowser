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
        var methodSymbol = model.GetDeclaredSymbol(resultSyntax);
        if(methodSymbol == null)
        {
            return default;
        }

        var fullMemberName = methodSymbol.GetFullMemberName() ?? string.Empty;
        return new MethodSyntaxModel() { methodFullName = fullMemberName, methodName = methodName };
    }
}


public record MethodSyntaxModel
{
    public string? methodName;
    public string? methodFullName;
}