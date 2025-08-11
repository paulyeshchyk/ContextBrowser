using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;

namespace RoslynKit.Model.Wrappers;

public record TypeSyntaxWrapper
{
    public ContextInfoElementType kind { get; private set; }

    public readonly ISymbol? Symbol;
    public readonly TypeDeclarationSyntax? Syntax;

    public string TypeFullName { get; private set; }

    public string SymbolName { get; private set; }

    public TypeSyntaxWrapper(ISymbol symbol, TypeDeclarationSyntax syntax)
    {
        Symbol = symbol;
        Syntax = syntax;
        TypeFullName = syntax.GetDeclarationName();
        SymbolName = symbol.GetFullMemberName();
        kind = syntax.GetContextInfoElementType();
    }

    public TypeSyntaxWrapper(ContextInfoElementType kind, string typeFullName, string symbolName)
    {
        this.kind = kind;
        TypeFullName = typeFullName;
        SymbolName = symbolName;
    }
}