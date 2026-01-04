using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit;
using RoslynKit.AWrappers;
using RoslynKit.Syntax;
using SemanticKit.Model.Options;

namespace RoslynKit.Syntax;

internal static class MemberDeclarationSyntaxExtensions
{
    private const string SFakeIdentifier = "FakeIdentifier";
    private const string SFakeName = "FakeName";

    public static string GetIdentifier(this SyntaxNode member) =>
        member switch
        {
            ClassDeclarationSyntax c => c.Identifier.Text,
            StructDeclarationSyntax s => s.Identifier.Text,
            RecordDeclarationSyntax r => r.Identifier.Text,
            EnumDeclarationSyntax e => e.Identifier.Text,
            MethodDeclarationSyntax m => m.Identifier.Text,
            BaseNamespaceDeclarationSyntax nn => nn.Name.ToFullString(),
            _ => SFakeIdentifier
        };

    public static string GetName(this SyntaxNode member) =>
        member switch
        {
            ClassDeclarationSyntax c => c.Identifier.Text,
            StructDeclarationSyntax s => s.Identifier.Text,
            RecordDeclarationSyntax r => r.Identifier.Text,
            EnumDeclarationSyntax e => e.Identifier.Text,
            MethodDeclarationSyntax m => m.Identifier.Text,
            BaseNamespaceDeclarationSyntax nn => nn.Name.ToFullString(),
            _ => SFakeName
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

    public static string GetNamespaceOrGlobal(this SyntaxNode availableSyntax)
    {
        var nameSpaceNodeSyntax = availableSyntax.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault();

        return nameSpaceNodeSyntax?.Name.ToString() ?? "Global";
    }
}
