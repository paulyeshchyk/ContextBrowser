using ContextBrowserKit.Log;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Basics.ContextKit;
using RoslynKit.Basics.Semantic;
using RoslynKit.Extensions;
using RoslynKit.Syntax.Parser.Wrappers;

namespace RoslynKit.Semantic.Builder;

public class CSharpDelegateContextInfoBuilder<TContext> : BaseContextInfoBuilder<TContext, DelegateDeclarationSyntax, SemanticModel>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpDelegateContextInfoBuilder(IContextCollector<TContext> collector, IContextFactory<TContext> factory, OnWriteLog? onWriteLog)
        : base(collector, factory, onWriteLog)
    {
    }

    protected override ISymInfoLoader BuildSymInfoDto(TContext? ownerContext, DelegateDeclarationSyntax delegateSyntax, SemanticModel semanticModel)
    {
        var symbol = semanticModel.GetDeclaredSymbol(delegateSyntax);
        if(symbol == null)
        {
            throw new ArgumentNullException($"Symbol for delegate not found: {delegateSyntax.Identifier}");
        }

        var NodeInfo = new CSharpSyntaxNodeWrapper(delegateSyntax);
        var SymInfo = new CSharpISymbolWrapper(symbol);

        var Name = SymInfo.GetName();
        var Namespace = delegateSyntax.GetNamespaceName();
        var SpanStart = delegateSyntax.Span.Start;
        var SpanEnd = delegateSyntax.Span.End;
        var Identifier = delegateSyntax.Identifier.Text;

        string FullName;
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
        return new SymInfoDto(ContextInfoElementType.@delegate, FullName, Name, Namespace, Identifier, SpanStart, SpanEnd, SymInfo, NodeInfo);
    }
}
