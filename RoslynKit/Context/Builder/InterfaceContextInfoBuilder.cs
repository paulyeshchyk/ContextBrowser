using ContextBrowser.ContextKit.Parser;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;

namespace RoslynKit.Context.Builder;

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
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Symbol for interface not found: {interfaceSyntax.Identifier.Text}");
            return default;
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Creating interface ContextInfo: {symbol.Name}");

        var result = _factory.Create(
            default,
            elementType: ContextInfoElementType.@interface,
            nsName: interfaceSyntax.GetNamespaceName(),
            name: symbol.Name,
            fullName: symbol.ToDisplayString(),
            syntaxNode: interfaceSyntax,
            spanStart: interfaceSyntax.Span.Start,
            spanEnd: interfaceSyntax.Span.End,
            symbol: symbol);

        if(result == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Creating interface ContextInfo failed: {symbol.Name}");
            return default;
        }

        _collector.Add(result);
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Created interface ContextInfo: {symbol.Name}");

        return result;
    }
}