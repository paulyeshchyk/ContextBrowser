using ContextBrowserKit.Log;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Basics.ContextKit;
using RoslynKit.Basics.Semantic;
using RoslynKit.Extensions;
using RoslynKit.Syntax.Parser.Wrappers;

namespace RoslynKit.Semantic.Builder;

public class CSharpInterfaceContextInfoBuilder<TContext> : BaseContextInfoBuilder<TContext, InterfaceDeclarationSyntax, SemanticModel>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpInterfaceContextInfoBuilder(IContextCollector<TContext> collector, IContextFactory<TContext> factory, OnWriteLog? onWriteLog)
        : base(collector, factory, onWriteLog)
    {
    }

    protected override ISymInfoLoader BuildSymInfoDto(TContext? ownerContext, InterfaceDeclarationSyntax interfaceSyntax, SemanticModel model)
    {
        ISymbol? symbol = null;
        try
        {
            symbol = model.GetDeclaredSymbol(interfaceSyntax);
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        var NodeInfo = new CSharpSyntaxNodeWrapper(interfaceSyntax);
        var SymInfo = new CSharpISymbolWrapper(symbol);

        var Namespace = interfaceSyntax.GetNamespaceName();
        var SpanStart = interfaceSyntax.Span.Start;
        var SpanEnd = interfaceSyntax.Span.End;
        var Identifier = interfaceSyntax.GetDeclarationName();

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
        return new SymInfoDto(ContextInfoElementType.@interface, FullName, Name, Namespace, Identifier, SpanStart, SpanEnd, SymInfo, NodeInfo);
    }
}