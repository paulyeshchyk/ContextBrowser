using ContextBrowser.ContextKit.Parser;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;

namespace RoslynKit.Syntax.Parser.ContextInfo;

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
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Symbol for delegate not found: {delegateSyntax.Identifier.Text}");
            return default;
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Creating delegate ContextInfo: {symbol.Name}");

        var result = _factory.Create(
            default, // У делегата может не быть родительского контекста
            elementType: ContextInfoElementType.@delegate,
            nsName: delegateSyntax.GetNamespaceName(),
            name: symbol.Name,
            fullName: symbol.ToDisplayString(),
            syntaxNode: delegateSyntax,
            spanStart: delegateSyntax.Span.Start,
            spanEnd: delegateSyntax.Span.End,
            symbol: symbol);

        if(result == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Creating delegate ContextInfo failed: {symbol.Name}");
            return default;
        }

        _collector.Add(result);
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Created delegate ContextInfo: {symbol.Name}");

        return result;
    }
}