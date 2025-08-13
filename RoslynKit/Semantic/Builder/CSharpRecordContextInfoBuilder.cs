using ContextBrowserKit.Log;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Basics.ContextKit;
using RoslynKit.Basics.Semantic;
using RoslynKit.Extensions;
using RoslynKit.Syntax.Parser.Wrappers;

namespace RoslynKit.Semantic.Builder;

public class CSharpRecordContextInfoBuilder<TContext> : BaseContextInfoBuilder<TContext, MemberDeclarationSyntax, SemanticModel>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpRecordContextInfoBuilder(IContextCollector<TContext> collector, IContextFactory<TContext> factory, OnWriteLog? onWriteLog)
        : base(collector, factory, onWriteLog)
    {
    }

    protected override ISymInfoLoader BuildSymInfoDto(TContext? ownerContext, MemberDeclarationSyntax recordSyntax, SemanticModel model)
    {
        ISymbol? symbol = null;
        try
        {
            symbol = model.GetDeclaredSymbol(recordSyntax);
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        var NodeInfo = new CSharpSyntaxNodeWrapper(recordSyntax);
        var SymInfo = new CSharpISymbolWrapper(symbol);

        var Namespace = recordSyntax.GetNamespaceName();
        var SpanStart = recordSyntax.Span.Start;
        var SpanEnd = recordSyntax.Span.End;
        var Identifier = recordSyntax.GetDeclarationName();

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
        return new SymInfoDto(ContextInfoElementType.record, FullName, Name, Namespace, Identifier, SpanStart, SpanEnd, SymInfo, NodeInfo);
    }
}
