using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Wrappers.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;

namespace RoslynKit.Semantic.Builder;

public class EnumContextInfoBuilder<TContext> : BaseContextInfoBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
    public EnumContextInfoBuilder(IContextCollector<TContext> collector, IContextFactory<TContext> factory, OnWriteLog? onWriteLog)
        : base(collector, factory, onWriteLog)
    {
    }

    public TContext? BuildContextInfoForEnum(EnumDeclarationSyntax enumSyntax, SemanticModel model)
    {
        ISymbol? symbol;

        try
        {
            symbol = model.GetDeclaredSymbol(enumSyntax);

            if(symbol == null)
            {
                _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Err, $"Symbol for enum not found: {enumSyntax.Identifier.Text}");
                return default;
            }
        }
        catch(Exception ex)
        {
            _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Err, $"Symbol for enum not found: {enumSyntax.Identifier.Text}; \n\n{ex}");
            return default;
        }

        _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Dbg, $"Creating enum ContextInfo: {symbol.Name}");

        var syntaxWrapper = new SyntaxNodeWrapper(enumSyntax);
        var symbolWrapper = new SymbolWrapper(symbol);

        var result = _factory.Create(
            owner: default, // У делегата может не быть родительского контекста
            elementType: ContextInfoElementType.@enum,
            nsName: enumSyntax.GetNamespaceName(),
            name: symbol.Name,
            fullName: symbol.ToDisplayString(),
            syntaxNode: syntaxWrapper,
            spanStart: enumSyntax.Span.Start,
            spanEnd: enumSyntax.Span.End,
            symbol: symbolWrapper);

        if(result == null)
        {
            _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Err, $"Creating enum ContextInfo failed: {symbol.Name}");
            return default;
        }

        _collector.Add(result);
        _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Dbg, $"Created delegate ContextInfo: {symbol.Name}");

        return result;
    }
}