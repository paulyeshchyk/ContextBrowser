using ContextBrowserKit.Log;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Basics.ContextKit;
using RoslynKit.Extensions;
using RoslynKit.Syntax.AssemblyLoader;
using RoslynKit.Syntax.Parser.Wrappers;

namespace RoslynKit.Semantic.Builder;

public class CSharpDelegateContextInfoBuilder<TContext> : BaseContextInfoBuilder<TContext, DelegateDeclarationSyntax, SemanticModel>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpDelegateContextInfoBuilder(IContextCollector<TContext> collector, IContextFactory<TContext> factory, OnWriteLog? onWriteLog)
        : base(collector, factory, onWriteLog)
    {
    }

    protected override IContextInfo BuildContextInfoDto(TContext? ownerContext, DelegateDeclarationSyntax syntax, SemanticModel model)
    {
        var symbol = CSharpSymbolLoader.LoadSymbol(syntax, model, _onWriteLog, CancellationToken.None);

        var syntaxWrap = new CSharpSyntaxNodeWrapper(syntax);
        var symbolWrap = new CSharpISymbolWrapper(symbol);

        var nameSpace = syntax.GetNamespaceName();
        var spanStart = syntax.Span.Start;
        var spanEnd = syntax.Span.End;
        var identifier = syntax.Identifier.Text;

        string name;
        string fullName;
        if (symbol != null)
        {
            fullName = symbol.ToDisplayString();
            name = symbolWrap.GetName();
        }
        else
        {
            fullName = $"{nameSpace}.{identifier}";
            name = identifier;
        }
        return new ContextInfoDto(ContextInfoElementType.@delegate, fullName, name, nameSpace, identifier, spanStart, spanEnd, symbol: symbolWrap, syntaxNode: syntaxWrap);
    }
}
