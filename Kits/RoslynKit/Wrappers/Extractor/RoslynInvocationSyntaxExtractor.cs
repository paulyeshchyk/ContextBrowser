using System;
using System.Threading;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using LoggerKit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;
using RoslynKit.Phases.Invocations;
using RoslynKit.Signature;
using RoslynKit.Wrappers.Meta;
using RoslynKit.Wrappers.Syntax;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Wrappers.Extractor;

//context: roslyn, builder
public class RoslynInvocationSyntaxExtractor : IInvocationSyntaxResolver
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly ISemanticInvocationResolver _semanticInvocationResolver;

    public RoslynInvocationSyntaxExtractor(ISemanticInvocationResolver semanticInvocationResolver, IAppLogger<AppLevel> logger)
    {
        _logger = logger;
        _semanticInvocationResolver = semanticInvocationResolver;
    }

    // context: roslyn, read
    public ISyntaxWrapper? ResolveInvocationSymbol(object invocation, SemanticOptions options, CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.R_Invocation, LogLevel.Dbg, $"Resolving symbol for invocation [{invocation}]", LogLevelNode.Start);

        if (invocation is not InvocationExpressionSyntax byInvocation)
        {
            throw new Exception("Invocation is not InvocationExpressionSyntax");
        }

        var result = GetSyntaxWrapper(byInvocation, options, cancellationToken);
        _logger.WriteLog(AppLevel.R_Invocation, LogLevel.Dbg, string.Empty, LogLevelNode.End);

        return result;
    }

    private ISyntaxWrapper? GetSyntaxWrapper(InvocationExpressionSyntax byInvocation, SemanticOptions options, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var invocationWrapper = new RoslynInvocationExpressionWrapper(byInvocation, _semanticInvocationResolver, _logger);
        var invocationSemanticModel = FindSemanticModel(invocationWrapper);
        if (invocationSemanticModel == null)
        {
            _logger.WriteLog(AppLevel.R_Invocation, LogLevel.Dbg, $"[MISS] Semantic model was not defined for [{invocationWrapper.Expression}]");
            return CSharpInvocationSyntaxWrapperConverter.FromExpression(byInvocation.Expression, options);
        }

        var symbol = RoslynMethodSymbolExtractor.GetMethodSymbol(invocationWrapper, invocationSemanticModel, _logger, cancellationToken);
        return (symbol != null)
            ? CSharpInvocationSyntaxWrapperConverter.FromSymbols(symbol, byInvocation)
            : CSharpInvocationSyntaxWrapperConverter.FromExpression(byInvocation.Expression, options);
    }

    // context: roslyn, read
    private ISemanticModelWrapper? FindSemanticModel(IInvocationNodeWrapper wrapper)
    {
        var treeWrapper = wrapper.BuildTree();
        if (treeWrapper == null)
        {
            _logger.WriteLog(AppLevel.R_Invocation, LogLevel.Err, $"[MISS]: Tree was not provided for invocation [{wrapper}]");

            return null;
        }

        return _semanticInvocationResolver.Resolve(treeWrapper);
    }
}