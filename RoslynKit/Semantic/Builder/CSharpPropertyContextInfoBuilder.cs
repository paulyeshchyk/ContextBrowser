using ContextBrowserKit.Log;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Basics.ContextKit;
using RoslynKit.Basics.Semantic;
using RoslynKit.Extensions;
using RoslynKit.Syntax.Parser.Wrappers;

namespace RoslynKit.Semantic.Builder;

public class CSharpPropertyContextInfoBuilder<TContext> : BaseContextInfoBuilder<TContext, PropertyDeclarationSyntax, SemanticModel>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpPropertyContextInfoBuilder(IContextCollector<TContext> collector, IContextFactory<TContext> factory, OnWriteLog? onWriteLog)
        : base(collector, factory, onWriteLog)
    {
    }

    protected override ISymInfoLoader BuildSymInfoDto(TContext? ownerContext, PropertyDeclarationSyntax propertySyntax, SemanticModel model)
    {
        ISymbol? symbol = null;
        try
        {
            symbol = model.GetDeclaredSymbol(propertySyntax);
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        var NodeInfo = new CSharpSyntaxNodeWrapper(propertySyntax);
        var SymInfo = new CSharpISymbolWrapper(symbol);

        var Namespace = propertySyntax.GetNamespaceName();
        var SpanStart = propertySyntax.Span.Start;
        var SpanEnd = propertySyntax.Span.End;
        var Identifier = propertySyntax.GetDeclarationName();

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
        return new SymInfoDto(ContextInfoElementType.property, FullName, Name, Namespace, Identifier, SpanStart, SpanEnd, SymInfo, NodeInfo);
    }
}