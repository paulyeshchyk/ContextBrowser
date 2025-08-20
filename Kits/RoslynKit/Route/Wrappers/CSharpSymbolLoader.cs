using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SemanticKit.Model;

namespace RoslynKit.Route.Wrappers;

public static class CSharpSymbolLoader
{
    public static ISymbol? LoadSymbol(MemberDeclarationSyntax? syntax, ISemanticModelWrapper model, OnWriteLog? onWriteLog, CancellationToken cancellationToken = default)
    {
        if(syntax == null)
        {
            return default;
        }

        try
        {
            var declaredSymbol = model.GetDeclaredSymbol(syntax, cancellationToken);
            if(declaredSymbol is ISymbol isymbol)
            {
                return isymbol;
            }
            else
            {
                throw new Exception("Symbol was not loaded for syntax");
            }
        }
        catch(Exception ex)
        {
            onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Exception, $"[CSharpSymbolLoader] {ex.Message}");
            onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Trace, $"{syntax.ToString()}");
            return default;
        }
    }
}
