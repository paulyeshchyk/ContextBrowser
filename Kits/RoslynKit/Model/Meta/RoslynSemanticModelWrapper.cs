using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using LoggerKit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Syntax;
using SemanticKit.Model;

namespace RoslynKit.Wrappers.Meta;

public class RoslynSemanticModelWrapper : ISemanticModelWrapper
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly SemanticModel _semanticModel;

    public RoslynSemanticModelWrapper(SemanticModel semanticModel, IAppLogger<AppLevel> logger)
    {
        _semanticModel = semanticModel;
        _logger = logger;
    }

    public async Task<object?> GetSymbolInfoAsync(object node, CancellationToken cancellationToken)
    {
        if (node is not SyntaxNode syntaxNode)
        {
            return null;
        }

        return await RoslynSymbolInfoManager.LoadSymbolInfoAsync(syntaxNode, _semanticModel, _logger, cancellationToken).ConfigureAwait(false);
    }

    public async Task<TSymbol?> GetDeclaredSymbolAsync<TSymbol>(object syntax, CancellationToken cancellationToken)
    {
        if (syntax is not MemberDeclarationSyntax syntaxNode)
        {
            return default;
        }

        return await RoslynSymbolInfoManager.LoadDeclaredSymbolForRootTreeAsync<TSymbol>(syntaxNode, _semanticModel, _logger, cancellationToken).ConfigureAwait(false);
    }

    public object? GetTypeInfo(object syntax)
    {
        if (syntax is not TypeSyntax syntaxNode)
        {
            return null;
        }
        return RoslynSymbolInfoManager.LoadTypeInfo(syntaxNode, _semanticModel, _logger);
    }
}

internal static class RoslynSymbolInfoManager
{
    public static Task<TSymbol?> LoadDeclaredSymbolForRootTreeAsync<TSymbol>(MemberDeclarationSyntax syntaxNode, SemanticModel semanticModel, IAppLogger<AppLevel> _logger, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var targetTree = syntaxNode.SyntaxTree;

        var root = targetTree.GetRoot(cancellationToken);

        var location = syntaxNode.Span;
        var node = root.FindNode(location);

        if (node == null)
        {
            TSymbol? result = default;
            return Task.FromResult(result);
        }

        var compilation = semanticModel.Compilation;
        var currentModel = compilation.GetSemanticModel(targetTree);
        return RoslynSymbolInfoManager.LoadDeclaredSymbolAsync<TSymbol>(syntaxNode, node, currentModel, _logger, cancellationToken);
    }

    public static Task<TSymbol?> LoadDeclaredSymbolAsync<TSymbol>(MemberDeclarationSyntax syntaxNode, SyntaxNode node, SemanticModel currentModel, IAppLogger<AppLevel> logger, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        TSymbol? result;
        try
        {
            result = (TSymbol?)currentModel.GetDeclaredSymbol(node, cancellationToken);
        }
        catch (Exception ex)
        {
            //FakeIdentifier
            var identifier = syntaxNode.GetIdentifier();
            var exText = $"Unable to get declared symbol for {identifier}, because of: {ex.Message}";
            logger.WriteLog(AppLevel.R_Syntax, LogLevel.Exception, exText);
            result = default;
        }
        return Task.FromResult(result);
    }

    public static Task<SymbolInfo?> LoadSymbolInfoAsync(SyntaxNode syntaxNode, SemanticModel semanticModel, IAppLogger<AppLevel> logger, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            SymbolInfo? result = semanticModel.GetSymbolInfo(syntaxNode, cancellationToken);
            return Task.FromResult(result);
        }
        catch (Exception ex)
        {
            logger.WriteLog(AppLevel.R_Syntax, LogLevel.Exception, ex.Message);
            return Task.FromResult((SymbolInfo?)null);
        }
    }

    public static ITypeSymbol? LoadTypeInfo(TypeSyntax syntaxNode, SemanticModel semanticModel, IAppLogger<AppLevel> _logger)
    {
        try
        {
            return semanticModel.GetTypeInfo(syntaxNode).Type;
        }
        catch (Exception ex)
        {
            _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Exception, ex.Message);
            return null;
        }
    }
}