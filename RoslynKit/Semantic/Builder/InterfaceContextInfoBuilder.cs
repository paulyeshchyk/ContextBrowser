using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Wrappers.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;

namespace RoslynKit.Semantic.Builder;

public class InterfaceContextInfoBuilder<TContext> : BaseContextInfoBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
    public InterfaceContextInfoBuilder(IContextCollector<TContext> collector, IContextFactory<TContext> factory, OnWriteLog? onWriteLog)
        : base(collector, factory, onWriteLog)
    {
    }

    public TContext? BuildContextInfoForInterface(InterfaceDeclarationSyntax interfaceSyntax, SemanticModel model)
    {
        var symbol = model.GetDeclaredSymbol(interfaceSyntax);
        if(symbol == null)
        {
            _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Err, $"Symbol for interface not found: {interfaceSyntax.Identifier.Text}");
            return default;
        }

        _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Dbg, $"Creating interface ContextInfo: {symbol.Name}");

        var syntaxWrapper = new SyntaxNodeWrapper(interfaceSyntax);
        var symbolWrapper = new SymbolWrapper(symbol);
        var result = _factory.Create(
            owner: default,
            elementType: ContextInfoElementType.@interface,
            nsName: interfaceSyntax.GetNamespaceName(),
            name: symbol.Name,
            fullName: symbol.ToDisplayString(),
            syntaxNode: syntaxWrapper,
            spanStart: interfaceSyntax.Span.Start,
            spanEnd: interfaceSyntax.Span.End,
            symbol: symbolWrapper);

        if(result == null)
        {
            _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Err, $"Creating interface ContextInfo failed: {symbol.Name}");
            return default;
        }

        _collector.Add(result);
        _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Dbg, $"Created interface ContextInfo: {symbol.Name}");

        return result;
    }
}