using System;
using System.Threading;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using LoggerKit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit;
using RoslynKit.Assembly;
using RoslynKit.AWrappers;
using RoslynKit.Syntax;
using SemanticKit.Model;

namespace RoslynKit.Assembly;

// context: roslyn, read
public static class RoslynSymbolLoader
{

    // context: roslyn, read
    public static ISymbol? LoadSymbol(MemberDeclarationSyntax? syntax, ISemanticModelWrapper model, IAppLogger<AppLevel> logger, CancellationToken cancellationToken)
    {
        if (syntax == null)
        {
            return default;
        }

        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var declaredSymbol = model.GetDeclaredSymbol(syntax, cancellationToken);
            return declaredSymbol as ISymbol ?? throw new Exception("Symbol was not loaded for syntax");
        }
        catch (Exception ex)
        {
            logger.WriteLog(AppLevel.R_Symbol, LogLevel.Exception, $"{syntax.GetIdentifier()} - {ex.Message}");
            logger.WriteLog(AppLevel.R_Symbol, LogLevel.Trace, $"{syntax}");
            return default;
        }
    }
}
