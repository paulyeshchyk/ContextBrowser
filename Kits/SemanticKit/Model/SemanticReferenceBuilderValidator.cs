using System.Linq;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;

namespace SemanticKit.Model;

public class SemanticReferenceBuilderValidator<TContext, TInvocationExpressionSyntax> : ISemanticReferenceBuilderValidator<TContext, TInvocationExpressionSyntax>
    where TContext : ContextInfo, IContextWithReferences<TContext>
    where TInvocationExpressionSyntax : class
{
    private readonly IAppLogger<AppLevel> _logger;

    public SemanticReferenceBuilderValidator(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Выполняет серию проверок и возвращает ValidationResult в случае успеха.
    /// </summary>
    /// <returns>Экземпляр ValidationResult с данными или null, если валидация не пройдена.</returns>
    public SemanticReferenceBuilderValidationResult<TContext, TInvocationExpressionSyntax>? Validate(TContext callerContext, IContextCollector<TContext> collector)
    {
        _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Dbg, $"Validating context [{callerContext.FullName}]");
        if (string.IsNullOrWhiteSpace(callerContext.FullName) || !collector.BySymbolDisplayName.TryGetValue(callerContext.FullName, out var callerContextInfo))
        {
            _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Warn, $"[MISS] Symbol not found in {collector.BySymbolDisplayName} for {callerContext.FullName}");
            return null;
        }

        var callerSyntaxNode = callerContext.SyntaxWrapper;
        if (callerSyntaxNode == null)
        {
            // срабатывает для функций, напр, nameof()
            _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Trace, $"[MISS] SyntaxNode is not defined for {callerContext.FullName}");
#warning to be checked twice
            return new SemanticReferenceBuilderValidationResult<TContext, TInvocationExpressionSyntax>(callerContextInfo, Enumerable.Empty<TInvocationExpressionSyntax>());
        }

        var canRaiseNoInvocationError = !(callerContext.ClassOwner?.ElementType == ContextInfoElementType.@class
            || callerContext.ClassOwner?.ElementType == ContextInfoElementType.@interface
            || callerContext.ClassOwner?.ElementType == ContextInfoElementType.@record);
        var invocationList = callerSyntaxNode.DescendantSyntaxNodes<TInvocationExpressionSyntax>();
        if (!invocationList.Any())
        {
            if (canRaiseNoInvocationError)
            {
                _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Warn, $"[MISS] Invocation not found for {callerContextInfo.FullName}");
            }
            else
            {
                _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Dbg, $"[SKIP] Invocation not found for {callerContextInfo.FullName}");
            }
        }
        else
        {
            _logger.WriteLog(AppLevel.R_Invocation, LogLevel.Dbg, $"[OK] Invocation was found for {callerContextInfo.FullName}");
        }

        return new SemanticReferenceBuilderValidationResult<TContext, TInvocationExpressionSyntax>(callerContextInfo, invocationList);
    }
}
