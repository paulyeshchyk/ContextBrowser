using ContextBrowserKit.Log;
using ContextKit.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;
using RoslynKit.Route.Wrappers;
using RoslynKit.Route.Wrappers.Syntax;
using SemanticKit.Model;

namespace RoslynKit.Route.Phases.ContextInfoBuilder;

public class CSharpDelegateContextInfoBuilder<TContext> : BaseContextInfoBuilder<TContext, DelegateDeclarationSyntax, ISemanticModelWrapper>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpDelegateContextInfoBuilder(IContextCollector<TContext> collector, IContextFactory<TContext> factory, OnWriteLog? onWriteLog)
        : base(collector, factory, onWriteLog)
    {
    }

    protected override IContextInfo BuildContextInfoDto(TContext? ownerContext, DelegateDeclarationSyntax syntax, ISemanticModelWrapper model, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var symbol = CSharpSymbolLoader.LoadSymbol(syntax, model, _onWriteLog, cancellationToken);

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
            fullName = symbolWrap.ToDisplayString();
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
