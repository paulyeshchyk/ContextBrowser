using System;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using LoggerKit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SemanticKit.Model;

namespace RoslynKit.Assembly;

public interface IRoslynSymbolLoader<TMemberDeclarationSyntax, TSymbol>
where TMemberDeclarationSyntax : MemberDeclarationSyntax
where TSymbol : class, ISymbol
{
    Task<TSymbol?> LoadSymbolAsync(TMemberDeclarationSyntax syntax, ISemanticModelWrapper model, CancellationToken cancellationToken);
}

// context: roslyn, read
public class RoslynSymbolLoader<TMemberDeclarationSyntax, TSymbol> : IRoslynSymbolLoader<TMemberDeclarationSyntax, TSymbol>
    where TMemberDeclarationSyntax : MemberDeclarationSyntax
    where TSymbol : class, ISymbol
{
    private readonly IAppLogger<AppLevel> _logger;

    public RoslynSymbolLoader(IAppLogger<AppLevel> logger) => _logger = logger;

    // context: roslyn, read
    public Task<TSymbol?> LoadSymbolAsync(TMemberDeclarationSyntax syntax, ISemanticModelWrapper model, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return model.GetDeclaredSymbolAsync<TSymbol>(syntax, cancellationToken);
    }
}
