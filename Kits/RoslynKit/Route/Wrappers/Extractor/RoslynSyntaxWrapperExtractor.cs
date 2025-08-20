using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Route.Wrappers.Syntax;

namespace RoslynKit.Route.Wrappers.Extractor;

internal static class RoslynSyntaxWrapperExtractor
{
    public static CSharpInvocationSyntaxWrapper? Extract(ExpressionSyntax resultSyntax, ISymbol? symbol, OnWriteLog? onWriteLog)
    {
        if(symbol == null)
        {
            onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Trace, $"[ERR]: Symbol not resolved for {resultSyntax}");
            return default;
        }

        return new CSharpInvocationSyntaxWrapper(symbol: symbol, syntax: resultSyntax);
    }
}