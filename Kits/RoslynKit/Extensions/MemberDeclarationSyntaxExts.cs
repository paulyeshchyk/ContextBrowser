using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SemanticKit.Model.Options;

namespace RoslynKit.Extensions;

internal static class MemberDeclarationSyntaxExts
{
    private const string SFakeDeclaration = "FakeDeclaration";

    public static string GetDeclarationName(this MemberDeclarationSyntax member) =>
        member switch
        {
            ClassDeclarationSyntax c => c.Identifier.Text,
            StructDeclarationSyntax s => s.Identifier.Text,
            RecordDeclarationSyntax r => r.Identifier.Text,
            EnumDeclarationSyntax e => e.Identifier.Text,
            MethodDeclarationSyntax m => m.Identifier.Text,
            BaseNamespaceDeclarationSyntax nn => nn.Name.ToFullString(),
            _ => SFakeDeclaration
        };

    public static SemanticAccessorModifierType? GetModifierType(this MethodDeclarationSyntax method)
    {
        if (method.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
            return SemanticAccessorModifierType.@public;
        if (method.Modifiers.Any(m => m.IsKind(SyntaxKind.ProtectedKeyword)))
            return SemanticAccessorModifierType.@protected;
        if (method.Modifiers.Any(m => m.IsKind(SyntaxKind.PrivateKeyword)))
            return SemanticAccessorModifierType.@private;
        if (method.Modifiers.Any(m => m.IsKind(SyntaxKind.InternalKeyword)))
            return SemanticAccessorModifierType.@internal;

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
