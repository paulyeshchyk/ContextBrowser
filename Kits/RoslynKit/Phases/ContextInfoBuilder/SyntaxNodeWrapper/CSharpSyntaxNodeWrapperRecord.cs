using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.AWrappers;

namespace RoslynKit.Phases.ContextInfoBuilder.SyntaxNodeWrapper;

public class CSharpSyntaxNodeWrapperRecord : CSharpSyntaxNodeWrapper<MemberDeclarationSyntax>
{
    private MemberDeclarationSyntax SyntaxNode => GetCoSyntax<MemberDeclarationSyntax>();

    public override string Identifier => SyntaxNode.GetIdentifier();

    public override string Namespace => SyntaxNode.GetNamespaceOrGlobal();

    public override string GetFullName() => $"{Namespace}.{GetName()}";

    public override string GetName() => SyntaxNode.GetIdentifier();

#warning this is incorrect
    public override string GetShortName() => GetName();
}