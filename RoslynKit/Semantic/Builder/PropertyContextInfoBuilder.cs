using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Wrappers.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;

namespace RoslynKit.Semantic.Builder;

public class PropertyContextInfoBuilder<TContext> : BaseContextInfoBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
    public PropertyContextInfoBuilder(IContextCollector<TContext> collector, IContextFactory<TContext> factory, OnWriteLog? onWriteLog)
        : base(collector, factory, onWriteLog)
    {
    }

    public TContext? BuildContextInfoForProperty(PropertyDeclarationSyntax propertySyntax, SemanticModel model, TContext? parentContext = default)
    {
        var symbol = model.GetDeclaredSymbol(propertySyntax);
        if(symbol == null)
        {
            _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Err, $"Symbol for property not found: {propertySyntax.Identifier.Text}");
            return default;
        }

        _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Dbg, $"Creating property ContextInfo: {symbol.Name}");

        var syntaxWrapper = new SyntaxNodeWrapper(propertySyntax);
        var symbolWrapper = new SymbolWrapper(symbol);
        var result = _factory.Create(
            owner: parentContext,
            elementType: ContextInfoElementType.@property,
            nsName: propertySyntax.GetNamespaceName(),
            name: symbol.Name,
            fullName: symbol.ToDisplayString(),
            syntaxNode: syntaxWrapper,
            spanStart: propertySyntax.Span.Start,
            spanEnd: propertySyntax.Span.End,
            symbol: symbolWrapper);

        if(result == null)
        {
            _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Err, $"Creating property ContextInfo failed: {symbol.Name}");
            return default;
        }

        _collector.Add(result);
        _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Dbg, $"Created property ContextInfo: {symbol.Name}");

        return result;
    }
}