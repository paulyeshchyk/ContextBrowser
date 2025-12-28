using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.AWrappers;

namespace RoslynKit.Model.SyntaxNodeWrapper;

public class CSharpRecordSyntaxNodeWrapper : CSharpSyntaxNodeWrapper<MemberDeclarationSyntax>
{
    private MemberDeclarationSyntax _syntaxNode => GetCoSyntax<MemberDeclarationSyntax>();

    public override string Identifier => _syntaxNode.GetIdentifier();

    public override string Namespace => _syntaxNode.GetNamespaceOrGlobal();

    public override string GetFullName() => $"{Namespace}.{GetName()}";

    public override string GetName() => _syntaxNode.GetIdentifier();

#warning this is incorrect
    public override string GetShortName() => GetName();
}