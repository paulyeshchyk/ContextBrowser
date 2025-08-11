using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Wrappers.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;

namespace RoslynKit.Semantic.Builder;

public class DelegateContextInfoBuilder<TContext> : BaseContextInfoBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
    public DelegateContextInfoBuilder(IContextCollector<TContext> collector, IContextFactory<TContext> factory, OnWriteLog? onWriteLog)
        : base(collector, factory, onWriteLog)
    {
    }

    public TContext? BuildContextInfoForDelegate(DelegateDeclarationSyntax delegateSyntax, SemanticModel model)
    {
        var symbol = model.GetDeclaredSymbol(delegateSyntax);
        if(symbol == null)
        {
            _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Err, $"Symbol for delegate not found: {delegateSyntax.Identifier.Text}");
            return default;
        }

        var syntaxWrapper = new SyntaxNodeWrapper(delegateSyntax);
        var symbolWrapper = new SymbolWrapper(symbol);
        _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Dbg, $"Creating delegate ContextInfo: {symbol.Name}");

        var result = _factory.Create(
            owner: default, // У делегата может не быть родительского контекста
            elementType: ContextInfoElementType.@delegate,
            nsName: delegateSyntax.GetNamespaceName(),
            name: symbol.Name,
            fullName: symbol.ToDisplayString(),
            syntaxNode: syntaxWrapper,
            spanStart: delegateSyntax.Span.Start,
            spanEnd: delegateSyntax.Span.End,
            symbol: symbolWrapper);

        if(result == null)
        {
            _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Err, $"Creating delegate ContextInfo failed: {symbol.Name}");
            return default;
        }

        _collector.Add(result);
        _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Dbg, $"Created delegate ContextInfo: {symbol.Name}");

        return result;
    }
}