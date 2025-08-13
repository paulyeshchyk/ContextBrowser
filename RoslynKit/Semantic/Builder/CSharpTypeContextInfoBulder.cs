using ContextBrowserKit.Log;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Basics.ContextKit;
using RoslynKit.Basics.Semantic;
using RoslynKit.Extensions;
using RoslynKit.Syntax.Parser.Wrappers;

namespace RoslynKit.Semantic.Builder;

public class CSharpTypeContextInfoBulder<TContext> : BaseContextInfoBuilder<TContext, MemberDeclarationSyntax, SemanticModel>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpTypeContextInfoBulder(IContextCollector<TContext> collector, IContextFactory<TContext> factory, OnWriteLog? onWriteLog)
        : base(collector, factory, onWriteLog)
    {
    }

    public TContext? BuildContextInfo(TContext? ownerContext, CSharpTypeSyntaxWrapper wrapper)
    {
        var symInfo = wrapper.GetSymInfoDto();

        return BuildContextInfo(ownerContext, symInfo);
    }

    protected override ISymInfoLoader BuildSymInfoDto(TContext? ownerContext, MemberDeclarationSyntax typeSyntax, SemanticModel model)
    {
        ISymbol? symbol = null;
        try
        {
            symbol = model.GetDeclaredSymbol(typeSyntax);
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        var NodeInfo = new CSharpSyntaxNodeWrapper(typeSyntax);
        var SymInfo = new CSharpISymbolWrapper(symbol);

        var Namespace = typeSyntax.GetNamespaceName();
        var SpanStart = typeSyntax.Span.Start;
        var SpanEnd = typeSyntax.Span.End;
        var Identifier = typeSyntax.GetDeclarationName();

        string FullName;
        string Name;
        if(symbol != null)
        {
            FullName = symbol.ToDisplayString();
            Name = SymInfo.GetName();
        }
        else
        {
            FullName = $"{Namespace}.{Identifier}";
            Name = Identifier;
        }
        return new SymInfoDto(ContextInfoElementType.@class, FullName, Name, Namespace, Identifier, SpanStart, SpanEnd, SymInfo, NodeInfo);
    }
}