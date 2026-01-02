using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.AWrappers;

namespace RoslynKit.Phases.ContextInfoBuilder.SyntaxNodeWrapper;

public class CSharpSyntaxNodeWrapperEnum : CSharpSyntaxNodeWrapper<EnumDeclarationSyntax>
{
    private MemberDeclarationSyntax _syntaxNode => GetCoSyntax<EnumDeclarationSyntax>();

    public override string Identifier => _syntaxNode.GetIdentifier();

    public override string Namespace => _syntaxNode.GetNamespaceOrGlobal();

    public override string GetName() => _syntaxNode.GetIdentifier();

    public override string GetFullName() => $"{Namespace}.{GetName()}";

    public override string GetShortName() => _syntaxNode.GetIdentifier();
}