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
    private readonly ImmutableArray<Diagnostic> _diagnostics;


    public RoslynSemanticModelWrapper(ImmutableArray<Diagnostic> diagnostics, SemanticModel semanticModel, IAppLogger<AppLevel> logger)
    {
        _semanticModel = semanticModel;
        _logger = logger;
        _diagnostics = diagnostics;
    }

#warning set object to SyntaxNode
    public async Task<object?> GetSymbolInfoAsync(object node, CancellationToken cancellationToken)
    {
        if (node is not SyntaxNode syntaxNode)
        {
            await Task.CompletedTask;
            return null;
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
            await Task.CompletedTask;
            return null;
        }
    }

#warning set object to MemberDeclarationSyntax
    public async Task<object?> GetDeclaredSymbolAsync(object syntax, CancellationToken cancellationToken)
    {
        if (syntax is not MemberDeclarationSyntax syntaxNode)
        {
            await Task.CompletedTask;
            return null;
        }

        var targetTree = syntaxNode.SyntaxTree;
        var compilation = _semanticModel.Compilation;
        var location = syntaxNode.Span;

        var root = targetTree.GetRoot(cancellationToken);

        var node = root.FindNode(location);
        if (node == null)
        {
            await Task.CompletedTask;
            return null;
        }

        var currentModel = compilation.GetSemanticModel(targetTree);

        try
        {
            var result = currentModel.GetDeclaredSymbol(node, cancellationToken);
            await Task.CompletedTask;

            return result;
        }
        catch (Exception ex)
        {
            //FakeIdentifier
            var identifier = syntaxNode.GetIdentifier();
            var exText = $"Unable to get declared symbol for {identifier}, because of: {ex.Message}";
            _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Exception, exText);

            await Task.CompletedTask;
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

    public ImmutableArray<Diagnostic> GetDiagnostics() => _diagnostics;

    public object GetSemanticModel() => _semanticModel;
}