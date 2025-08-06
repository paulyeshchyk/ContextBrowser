using ContextBrowser.ContextKit.Parser;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Model.Wrappers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RoslynKit.Context.Builder;

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
        var typemodel = TypeSyntaxExtractor.Extract(callerSyntaxNode, model);
        if(typemodel == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Syntax \"{callerSyntaxNode}\" was not resolved in {nsName}");
            return default;
        }

        return BuildContextInfoForType(typemodel, nsName, spanStart, spanEnd, callerSyntaxNode, symbol);
    }

    public TContext? BuildContextInfoForType(TypeSyntaxWrapper typemodel, string nsName, int spanStart = 0, int spanEnd = 0, TypeDeclarationSyntax? callerSyntaxNode = null, ISymbol? symbol = null)
    {
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Creating type ContextInfo [{typemodel.TypeFullName}]");

        var result = _factory.Create(default, typemodel.kind, nsName, typemodel.TypeFullName, typemodel.SymbolName, callerSyntaxNode, spanStart, spanEnd, symbol);
        if(result == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Creating type ContextInfo failed {typemodel.TypeFullName}");
            return default;
        }

        _collector.Add(result);

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Created type ContextInfo: {typemodel.TypeFullName}");
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Trace, $"Created type ContextInfo: \r\n{JsonSerializer.Serialize(result, options: new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.Preserve, WriteIndented = true })}");

        return result;
    }
}
