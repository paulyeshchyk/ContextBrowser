using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Wrappers.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Model.Wrappers;

namespace RoslynKit.Semantic.Builder;

public class TypeContextInfoBulder<TContext> : BaseContextInfoBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
    public TypeContextInfoBulder(IContextCollector<TContext> collector, IContextFactory<TContext> factory, OnWriteLog? onWriteLog)
        : base(collector, factory, onWriteLog)
    {
    }

    public TContext? BuildContextInfoForType(TypeDeclarationSyntax callerSyntaxNode, SemanticModel model, string nsName, ISymbol? symbol)
    {
        var spanStart = callerSyntaxNode.Span.Start;
        var spanEnd = callerSyntaxNode.Span.End;

        TypeSyntaxWrapper? typemodel = null;

        try
        {
            typemodel = TypeSyntaxExtractor.Extract(callerSyntaxNode, model);
            if(typemodel == null)
            {
                _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Err, $"Syntax \"{callerSyntaxNode}\" was not resolved in {nsName}");
                return default;
            }
        }
        catch(Exception ex)
        {
            _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Err, $"Exception due symbol extracting {callerSyntaxNode}", LogLevelNode.Start);
            _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Err, $"{ex}");
            _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Err, string.Empty, LogLevelNode.End);
            return default;
        }


        return BuildContextInfoForType(typemodel, nsName, spanStart, spanEnd, callerSyntaxNode, symbol);
    }

    public TContext? BuildContextInfoForType(TypeSyntaxWrapper typemodel, string nsName, int spanStart = 0, int spanEnd = 0, TypeDeclarationSyntax? callerSyntaxNode = null, ISymbol? symbol = null)
    {
        _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Dbg, $"Creating type ContextInfo [{typemodel.TypeFullName}]");

        var syntaxWrapper = new SyntaxNodeWrapper(callerSyntaxNode);
        var symbolWrapper = new SymbolWrapper(symbol);
        var result = _factory.Create(
            owner: default,
            elementType: typemodel.kind,
            nsName: nsName,
            name: typemodel.TypeFullName,
            fullName: typemodel.SymbolName,
            syntaxNode: syntaxWrapper,
            spanStart: spanStart,
            spanEnd: spanEnd,
            symbol: symbolWrapper
            );

        if(result == null)
        {
            _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Err, $"Creating type ContextInfo failed {typemodel.TypeFullName}");
            return default;
        }

        _collector.Add(result);

        _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Dbg, $"Created type ContextInfo: {typemodel.TypeFullName}");

        return result;
    }
}