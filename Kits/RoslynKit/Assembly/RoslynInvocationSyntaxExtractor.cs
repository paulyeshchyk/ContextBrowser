using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using LoggerKit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit;
using RoslynKit.Assembly;
using RoslynKit.Converters;
using RoslynKit.Model.Meta;
using RoslynKit.Wrappers;
using RoslynKit.Wrappers.Meta;
using SemanticKit.Model;
using SemanticKit.Model.Options;
using SemanticKit.Model.SyntaxWrapper;

namespace RoslynKit.Assembly;

//context: roslyn, read
public class RoslynInvocationSyntaxExtractor : IInvocationSyntaxResolver
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly ISemanticInvocationResolver<RoslynSyntaxTreeWrapper> _semanticInvocationResolver;
    private readonly ICSharpInvocationSyntaxWrapperConverter _invocationSyntaxWrapperConverter;

    public RoslynInvocationSyntaxExtractor(ISemanticInvocationResolver<RoslynSyntaxTreeWrapper> semanticInvocationResolver, IAppLogger<AppLevel> logger, ICSharpInvocationSyntaxWrapperConverter invocationSyntaxWrapperConverter)
    {
        _logger = logger;
        _semanticInvocationResolver = semanticInvocationResolver;
        _invocationSyntaxWrapperConverter = invocationSyntaxWrapperConverter;
    }

    // context: roslyn, read
    public async Task<ISyntaxWrapper?> ResolveInvocationSymbolAsync(object invocation, SemanticOptions options, CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.R_Invocation, LogLevel.Dbg, $"Resolving symbol for invocation [{invocation}]", LogLevelNode.Start);

        if (invocation is not InvocationExpressionSyntax byInvocation)
        {
            throw new Exception("Invocation is not InvocationExpressionSyntax");
        }

        var result = await GetSyntaxWrapperAsync(byInvocation, options, cancellationToken).ConfigureAwait(false);
        _logger.WriteLog(AppLevel.R_Invocation, LogLevel.Dbg, string.Empty, LogLevelNode.End);

        return result;
    }

    // context: roslyn, read
    internal async Task<ISyntaxWrapper?> GetSyntaxWrapperAsync(InvocationExpressionSyntax byInvocation, SemanticOptions options, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var invocationWrapper = new RoslynInvocationExpressionWrapper(byInvocation, _semanticInvocationResolver, _logger);
        var invocationSemanticModel = FindSemanticModel(invocationWrapper);
        if (invocationSemanticModel == null)
        {
            _logger.WriteLog(AppLevel.R_Invocation, LogLevel.Dbg, $"[MISS] Semantic model was not defined for [{invocationWrapper.Expression}]");
            return _invocationSyntaxWrapperConverter.FromExpression(byInvocation.Expression, options);
        }

        var symbol = await RoslynInvocationSyntaxExtractor.GetMethodSymbolAsync(invocationWrapper, invocationSemanticModel, _logger, cancellationToken).ConfigureAwait(false);
        return (symbol != null)
            ? _invocationSyntaxWrapperConverter.FromSymbols(byInvocation, symbol)
            : _invocationSyntaxWrapperConverter.FromExpression(byInvocation.Expression, options);
    }

    // context: roslyn, read
    internal ISemanticModelWrapper? FindSemanticModel(IInvocationNodeWrapper<RoslynSyntaxTreeWrapper> wrapper)
    {
        var treeWrapper = wrapper.BuildTree();

        return _semanticInvocationResolver.Resolve(treeWrapper);
    }

    // context: roslyn, read
    internal static async Task<IMethodSymbol?> GetMethodSymbolAsync(IInvocationNodeWrapper<RoslynSyntaxTreeWrapper> invocation, ISemanticModelWrapper semanticModel, IAppLogger<AppLevel>? logger, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        logger?.WriteLog(AppLevel.R_Invocation, LogLevel.Dbg, $"Looking IMethodSymbol for expression: {invocation.Expression}");

        var si = await semanticModel.GetSymbolInfoAsync(invocation.Expression, cancellationToken);
        if (si is not SymbolInfo symbolInfo)
        {
            logger?.WriteLog(AppLevel.R_Invocation, LogLevel.Exception, $"SymbolInfo not found for {invocation.Expression}");
            return null;
        }

        if (symbolInfo.Symbol is IMethodSymbol method)
        {
            logger?.WriteLog(AppLevel.R_Invocation, LogLevel.Dbg, $"[DONE] Found IMethodSymbol for expression: {invocation.Expression}");
            return method;
        }

        if (symbolInfo.CandidateSymbols.Length > 0)
        {
            logger?.WriteLog(AppLevel.R_Invocation, LogLevel.Dbg, $"[DONE] Found Candidate of IMethodSymbol for expression: {invocation.Expression}");
            return symbolInfo.CandidateSymbols.OfType<IMethodSymbol>().FirstOrDefault();
        }
        logger?.WriteLog(AppLevel.R_Invocation, LogLevel.Warn, $"[FAIL]: No symbol for expression: {invocation.Expression}");
        return null;
    }

}