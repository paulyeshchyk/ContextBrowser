using ContextKit.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.AWrappers;

namespace RoslynKit.Model.SyntaxNodeWrapper;

public class CSharpPropertySyntaxNodeWrapper : CSharpSyntaxNodeWrapper<PropertyDeclarationSyntax>, ISymbolInfo
{
    private PropertyDeclarationSyntax _syntaxNode => GetCoSyntax<PropertyDeclarationSyntax>();

    public override string Identifier => _syntaxNode.GetIdentifier();

    public override string Namespace => _syntaxNode.GetNamespaceOrGlobal();

    public override string GetFullName() => $"{Namespace}.{GetName()}";

    public override string GetName() => _syntaxNode.GetIdentifier();

#warning this is incorrect
    public override string GetShortName() => GetName();
}