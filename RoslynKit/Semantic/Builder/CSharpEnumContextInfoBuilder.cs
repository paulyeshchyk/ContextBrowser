using ContextBrowserKit.Log;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Basics.ContextKit;
using RoslynKit.Basics.Semantic;
using RoslynKit.Extensions;
using RoslynKit.Syntax.Parser.Wrappers;

namespace RoslynKit.Semantic.Builder;

public class CSharpEnumContextInfoBuilder<TContext> : BaseContextInfoBuilder<TContext, EnumDeclarationSyntax, SemanticModel>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpEnumContextInfoBuilder(IContextCollector<TContext> collector, IContextFactory<TContext> factory, OnWriteLog? onWriteLog)
        : base(collector, factory, onWriteLog)
    {
    }

    protected override ISymInfoLoader BuildSymInfoDto(TContext? ownerContext, EnumDeclarationSyntax enumSyntax, SemanticModel model)
    {
        ISymbol? symbol = null;
        try
        {
            symbol = model.GetDeclaredSymbol(enumSyntax);
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        var NodeInfo = new CSharpSyntaxNodeWrapper(enumSyntax);
        var SymInfo = new CSharpISymbolWrapper(symbol);

        var Name = SymInfo.GetName();
        var Namespace = enumSyntax.GetNamespaceName();
        var SpanStart = enumSyntax.Span.Start;
        var SpanEnd = enumSyntax.Span.End;
        var Identifier = enumSyntax.Identifier.Text;

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
        return new SymInfoDto(ContextInfoElementType.@enum, FullName, Name, Namespace, Identifier, SpanStart, SpanEnd, SymInfo, NodeInfo);
    }
}
