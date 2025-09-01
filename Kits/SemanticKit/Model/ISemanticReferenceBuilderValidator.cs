using System.Collections.Generic;
using ContextKit.Model;

namespace SemanticKit.Model;

public interface ISemanticReferenceBuilderValidator<TContext, TInvocationExpressionSyntax>
    where TContext : ContextInfo, IContextWithReferences<TContext>
    where TInvocationExpressionSyntax : class
{
    SemanticReferenceBuilderValidationResult<TContext, TInvocationExpressionSyntax>? Validate(TContext callerContext, IContextCollector<TContext> collector);
}

/// <summary>
/// Результат валидации, содержащий необходимые данные для дальнейшей работы.
/// </summary>
public class SemanticReferenceBuilderValidationResult<TContext, TInvocationExpressionSyntax>
{
    public TContext CallerContextInfo { get; }

    public IEnumerable<TInvocationExpressionSyntax> Invocations { get; }

    public SemanticReferenceBuilderValidationResult(TContext callerContextInfo, IEnumerable<TInvocationExpressionSyntax> invocations)
    {
        CallerContextInfo = callerContextInfo;
        Invocations = invocations;
    }
}
