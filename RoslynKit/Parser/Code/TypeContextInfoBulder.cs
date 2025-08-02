using ContextBrowser.ContextKit.Parser;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Parser.Extractor;

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

    public TContext? BuildContextInfoForType(MemberDeclarationSyntax callerSyntaxNode, SemanticModel model, string nsName)
    {
        var typemodel = TypeSyntaxExtractor.Extract(callerSyntaxNode, model);
        if(typemodel == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Syntax \"{callerSyntaxNode}\" was not resolved in {nsName}");
            return default;
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Info, $"[{typemodel.typeFullName}]:Creating type ContextInfo");

        var result = _factory.Create(default, typemodel.kind, nsName, typemodel.typeFullName, null, callerSyntaxNode);
        _collector.Add(result);

        return result;
    }
}
