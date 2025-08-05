using ContextBrowser.ContextKit.Parser;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Model.Wrappers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RoslynKit.Parser.Phases;

public class TypeContextInfoBulder<TContext>
        where TContext : IContextWithReferences<TContext>
{
    private IContextCollector<TContext> _collector;
    private IContextFactory<TContext> _factory;
    private OnWriteLog? _onWriteLog = null;

    public TypeContextInfoBulder(IContextCollector<TContext> collector, IContextFactory<TContext> factory, OnWriteLog? onWriteLog)
    {
        _collector = collector;
        _factory = factory;
        _onWriteLog = onWriteLog;
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
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Creating type ContextInfo [{typemodel.TypeFullName}]", LogLevelNode.Start);

        var result = _factory.Create(default, typemodel.kind, nsName, typemodel.TypeFullName, typemodel.SymbolName, callerSyntaxNode, spanStart, spanEnd, symbol);
        if(result != null)
        {
            _collector.Add(result);

            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Created type ContextInfo: {typemodel.TypeFullName}");
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Info, $"Created type ContextInfo: \r\n{JsonSerializer.Serialize(result, options: new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.Preserve, WriteIndented = true })}");
        }
        else
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Creating method ContextInfo failed {typemodel.TypeFullName}");
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, string.Empty, LogLevelNode.End);

        return result;
    }
}
