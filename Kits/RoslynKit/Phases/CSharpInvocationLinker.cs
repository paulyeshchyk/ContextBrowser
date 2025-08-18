using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Extensions;
using ContextKit.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Model;
using RoslynKit.Syntax.Parser.Extractor;

namespace RoslynKit.Phases;

public class CSharpInvocationLinker<TContext>
    where TContext : ContextInfo, IContextWithReferences<TContext>
{
    private RoslynPhaseParserInvocationLinksBuilder<TContext> _linksInvocationBuilder;
    private OnWriteLog? _onWriteLog;
    private CSharpInvocationSyntaxExtractor _invocationSyntaxExtractor;
    private RoslynCodeParserOptions _options;

    public CSharpInvocationLinker(RoslynPhaseParserInvocationLinksBuilder<TContext> linksInvocationBuilder, OnWriteLog? onWriteLog, CSharpInvocationSyntaxExtractor invocationSyntaxExtractor, RoslynCodeParserOptions options)
    {
        _linksInvocationBuilder = linksInvocationBuilder;
        _onWriteLog = onWriteLog;
        _invocationSyntaxExtractor = invocationSyntaxExtractor;
        _options = options;
    }

    public void Link(IEnumerable<InvocationExpressionSyntax> invocationList, TContext callerContext, TContext callerContextInfo, CancellationToken cancellationToken)
    {
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Resolving invocations for [{callerContext.GetDebugSymbolName()}]", LogLevelNode.Start);
        foreach(var invocation in invocationList)
        {
            ResolveSymbolThenLink(invocation, callerContextInfo, _linksInvocationBuilder, cancellationToken);
        }
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }

    private void ResolveSymbolThenLink(InvocationExpressionSyntax invocation, TContext callerContextInfo, RoslynPhaseParserInvocationLinksBuilder<TContext> linksInvocationBuilder, CancellationToken cancellationToken)
    {
        var symbolDto = _invocationSyntaxExtractor.ResolveSymbol(invocation, cancellationToken);
        if(symbolDto == null)
        {
            //TODO: warn
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[FAIL]: Symbol was not found [{invocation}]");
            return;
        }

        linksInvocationBuilder.LinkInvocation(callerContextInfo, symbolDto, _options);
    }
}
