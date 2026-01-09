using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;

namespace SemanticKit.Parsers.Strategy.Invocation;

public interface IInvocationBuilderValidator<TContext, TInvocationExpressionSyntax>
    where TContext : IContextWithReferences<TContext>
    where TInvocationExpressionSyntax : class
{
    InvocationBuilderValidatorResult<TContext, TInvocationExpressionSyntax>? Validate(TContext callerContext, IContextCollector<TContext> collector);
}

public class InvocationBuilderValidator<TContext, TInvocationExpressionSyntax> : IInvocationBuilderValidator<TContext, TInvocationExpressionSyntax>
    where TContext : IContextWithReferences<TContext>
    where TInvocationExpressionSyntax : class
{
    private readonly IAppLogger<AppLevel> _logger;

    public InvocationBuilderValidator(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Выполняет серию проверок и возвращает ValidationResult в случае успеха.
    /// </summary>
    /// <returns>Экземпляр ValidationResult с данными или null, если валидация не пройдена.</returns>
    public InvocationBuilderValidatorResult<TContext, TInvocationExpressionSyntax>? Validate(TContext callerContext, IContextCollector<TContext> collector)
    {
        _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Dbg, $"Validating context [{callerContext.FullName}]");
        if (string.IsNullOrWhiteSpace(callerContext.FullName) || !collector.Collection.TryGetValue(callerContext.FullName, out var callerContextInfo))
        {
            _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Warn, $"[MISS] Symbol not found in {collector.Collection} for {callerContext.FullName}");
            return null;
        }

        var callerSyntaxNode = callerContext.SyntaxWrapper;
        if (callerSyntaxNode == null)
        {
            // срабатывает для функций, напр, nameof()
            _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Trace, $"[MISS] SyntaxNode is not defined for {callerContext.FullName}");
#warning to be checked twice
            return new InvocationBuilderValidatorResult<TContext, TInvocationExpressionSyntax>(callerContextInfo, []);
        }

        var canRaiseNoInvocationError = !(callerContext.ClassOwner?.ElementType.IsEntityDefinition() ?? false);

        var invocationList = callerSyntaxNode.DescendantSyntaxNodes<TInvocationExpressionSyntax>().ToList();
        if (invocationList.Count == 0)
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

        return new InvocationBuilderValidatorResult<TContext, TInvocationExpressionSyntax>(callerContextInfo, invocationList);
    }
}

/// <summary>
/// Результат валидации, содержащий необходимые данные для дальнейшей работы.
/// </summary>
public class InvocationBuilderValidatorResult<TContext, TInvocationExpressionSyntax>
{
    public TContext CallerContextInfo { get; }

    public IEnumerable<TInvocationExpressionSyntax> Invocations { get; }

    public InvocationBuilderValidatorResult(TContext callerContextInfo, IEnumerable<TInvocationExpressionSyntax> invocations)
    {
        CallerContextInfo = callerContextInfo;
        Invocations = invocations;
    }
}
