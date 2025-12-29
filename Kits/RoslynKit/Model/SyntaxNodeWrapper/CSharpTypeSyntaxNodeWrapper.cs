using ContextKit.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.AWrappers;

namespace RoslynKit.Model.SyntaxNodeWrapper;

public class CSharpTypeSyntaxNodeWrapper : CSharpSyntaxNodeWrapper<MemberDeclarationSyntax>, ISymbolInfo
{
    private MemberDeclarationSyntax _syntaxNode => GetCoSyntax<MemberDeclarationSyntax>();

    public override string Identifier => _syntaxNode.GetIdentifier();

    public override string Namespace => _syntaxNode.GetNamespaceOrGlobal();

    public override string GetFullName() => $"{Namespace}.{GetName()}";

    public override string GetName() => _syntaxNode.GetIdentifier();

    public override string GetShortName() => GetName();
}