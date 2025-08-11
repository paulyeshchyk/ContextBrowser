using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Model;

namespace RoslynKit.Extensions;

internal static class MemberDeclarationSyntaxExts
{
    private const string SFakeDeclaration = "FakeDeclaration";

    public static ContextInfoElementType GetContextInfoElementType(this MemberDeclarationSyntax? syntax)
    {
        return syntax switch
        {
            ClassDeclarationSyntax => ContextInfoElementType.@class,
            StructDeclarationSyntax => ContextInfoElementType.@struct,
            RecordDeclarationSyntax => ContextInfoElementType.@record,
            EnumDeclarationSyntax => ContextInfoElementType.@enum,
            _ => ContextInfoElementType.none
        };
    }

    public static string GetDeclarationName(this MemberDeclarationSyntax member) =>
        member switch
        {
            ClassDeclarationSyntax c => c.Identifier.Text,
            StructDeclarationSyntax s => s.Identifier.Text,
            RecordDeclarationSyntax r => r.Identifier.Text,
            EnumDeclarationSyntax e => e.Identifier.Text,
            BaseNamespaceDeclarationSyntax nn => nn.Name.ToFullString(),
            _ => SFakeDeclaration
        };

    public static RoslynCodeParserAccessorModifierType? GetModifierType(this MethodDeclarationSyntax method)
    {
        if(method.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
            return RoslynCodeParserAccessorModifierType.@public;
        if(method.Modifiers.Any(m => m.IsKind(SyntaxKind.ProtectedKeyword)))
            return RoslynCodeParserAccessorModifierType.@protected;
        if(method.Modifiers.Any(m => m.IsKind(SyntaxKind.PrivateKeyword)))
            return RoslynCodeParserAccessorModifierType.@private;
        if(method.Modifiers.Any(m => m.IsKind(SyntaxKind.InternalKeyword)))
            return RoslynCodeParserAccessorModifierType.@internal;

        return null;
    }

    public static string GetNamespaceName(this MemberDeclarationSyntax availableSyntax)
    {
        var nameSpaceNodeSyntax = availableSyntax
            .Ancestors()
            .OfType<BaseNamespaceDeclarationSyntax>()
            .FirstOrDefault();

        return nameSpaceNodeSyntax?.Name.ToString() ?? "Global";
    }
}
