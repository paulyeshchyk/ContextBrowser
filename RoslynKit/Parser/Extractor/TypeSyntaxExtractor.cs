using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Parser.Phases;

namespace RoslynKit.Parser.Extractor;

public static class TypeSyntaxExtractor
{
    public static TypeSyntaxModel? Extract(MemberDeclarationSyntax? resultSyntax, SemanticModel model)
    {
        if(resultSyntax == null)
        {
            return default;
        }

        var kind = resultSyntax.GetContextInfoElementType();
        var typeName = resultSyntax.GetDeclarationName();

        return new TypeSyntaxModel() { typeFullName = typeName, kind = kind };
    }
}

public record TypeSyntaxModel
{
    public ContextInfoElementType kind { get; set; }

    public string typeFullName { get; set; } = string.Empty;
}