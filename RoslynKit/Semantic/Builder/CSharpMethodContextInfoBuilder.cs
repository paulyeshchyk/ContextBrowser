using ContextBrowserKit.Log;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Basics.ContextKit;
using RoslynKit.Basics.Semantic;
using RoslynKit.Extensions;
using RoslynKit.Syntax.Parser.Wrappers;

namespace RoslynKit.Semantic.Builder;

// context: csharp, build, contextInfo
public class CSharpMethodContextInfoBuilder<TContext> : BaseContextInfoBuilder<TContext, MethodDeclarationSyntax, SemanticModel>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpMethodContextInfoBuilder(IContextCollector<TContext> collector, IContextFactory<TContext> factory, OnWriteLog? onWriteLog)
        : base(collector, factory, onWriteLog)
    {
    }

    public TContext? BuildContextInfo(TContext? ownerContext, CSharpMethodSyntaxWrapper wrapper)
    {
        var symInfo = wrapper.GetSymInfoDto();

        return BuildContextInfo(ownerContext, symInfo);
    }

    protected override ISymInfoLoader BuildSymInfoDto(TContext? ownerContext, MethodDeclarationSyntax syntaxNode, SemanticModel model)
    {
        ISymbol? symbol = null;
        try
        {
            symbol = model.GetDeclaredSymbol(syntaxNode);
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        var NodeInfo = new CSharpSyntaxNodeWrapper(syntaxNode);
        var SymInfo = new CSharpISymbolWrapper(symbol);

        var Name = SymInfo.GetName();
        var Namespace = syntaxNode.GetNamespaceName();
        var SpanStart = syntaxNode.Span.Start;
        var SpanEnd = syntaxNode.Span.End;
        var Identifier = syntaxNode.Identifier.Text;

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
        return new SymInfoDto(ContextInfoElementType.method, FullName, Name, Namespace, Identifier, SpanStart, SpanEnd, SymInfo, NodeInfo);
    }
}