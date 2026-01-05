using System;
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

#warning set object to SyntaxNode
    public async Task<object?> GetSymbolInfoAsync(object node, CancellationToken cancellationToken)
    {
        if (node is not SyntaxNode syntaxNode)
        {
            return Task.FromResult<object?>(null);
        }
        try
        {
            var result = _semanticModel.GetSymbolInfo(syntaxNode, cancellationToken);
            await Task.CompletedTask;
            return result;
        }
        catch (Exception ex)
        {
            _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Exception, ex.Message);
            return Task.FromResult<object?>(null);
        }
    }

#warning set object to MemberDeclarationSyntax
    public object? GetDeclaredSymbol(object syntax, CancellationToken cancellationToken)
    {
        if (syntax is not MemberDeclarationSyntax syntaxNode)
        {
            return null;
        }
        try
        {
            return _semanticModel.GetDeclaredSymbol(syntaxNode, cancellationToken);
        }
        catch (Exception ex)
        {
            var exText = $"Unable to get declared symbol for {syntaxNode.GetIdentifier()}, because of: {ex.Message}";
            _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Exception, exText);
            return null;
        }
    }

#warning set object to SyntaxNode
    public object? GetSymbolForInvocation(object invocationNode)
    {
        if (invocationNode is not SyntaxNode syntaxNode)
        {
            return null;
        }
        try
        {
            var symbolInfo = _semanticModel.GetSymbolInfo(syntaxNode);
            return symbolInfo.Symbol;
        }
        catch (Exception ex)
        {
            _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Exception, ex.Message);
            return null;
        }
    }

#warning set object to TypeSyntax
    public object? GetTypeInfo(object syntax)
    {
        if (syntax is not TypeSyntax syntaxNode)
        {
            return null;
        }

        try
        {
            return _semanticModel.GetTypeInfo(syntaxNode).Type;
        }
        catch (Exception ex)
        {
            _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Exception, ex.Message);
            return null;
        }
    }
}