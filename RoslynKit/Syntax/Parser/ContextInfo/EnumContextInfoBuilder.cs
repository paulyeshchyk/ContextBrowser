using ContextBrowser.ContextKit.Parser;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;

namespace RoslynKit.Syntax.Parser.ContextInfo;

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

        var result = _factory.Create(
            default, // У делегата может не быть родительского контекста
            elementType: ContextInfoElementType.@enum,
            nsName: enumSyntax.GetNamespaceName(),
            name: symbol.Name,
            fullName: symbol.ToDisplayString(),
            syntaxNode: enumSyntax,
            spanStart: enumSyntax.Span.Start,
            spanEnd: enumSyntax.Span.End,
            symbol: symbol);

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