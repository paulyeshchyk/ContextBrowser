using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynKit.Syntax.AssemblyLoader;

public static class CSharpSymbolLoader
{
    public static ISymbol? LoadSymbol(MemberDeclarationSyntax? syntax, SemanticModel model, OnWriteLog? onWriteLog, CancellationToken cancellationToken = default)
    {
        if(syntax == null)
        {
            return default;
        }

        try
        {
            return model.GetDeclaredSymbol(syntax, cancellationToken);
        }
        catch(Exception ex)
        {
            onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Exception, $"[CSharpSymbolLoader] {ex.Message}");
            onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Trace, $"{syntax.ToString()}");
            return default;
        }
    }
}
