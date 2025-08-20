using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Extensions;
using ContextKit.Model;

namespace RoslynKit.Route.Phases.Invocations;

public class ReferenceBuilderValidator<TContext, TInvocationExpressionSyntax>
    where TContext : ContextInfo, IContextWithReferences<TContext>
    where TInvocationExpressionSyntax : class
{
    private readonly OnWriteLog? _onWriteLog;

    public ReferenceBuilderValidator(OnWriteLog? onWriteLog)
    {
        _onWriteLog = onWriteLog;
    }

    /// <summary>
    /// Результат валидации, содержащий необходимые данные для дальнейшей работы.
    /// </summary>
    public class ValidationResult
    {
        public TContext CallerContextInfo { get; }

        public IEnumerable<TInvocationExpressionSyntax> Invocations { get; }

        public ValidationResult(TContext callerContextInfo, IEnumerable<TInvocationExpressionSyntax> invocations)
        {
            CallerContextInfo = callerContextInfo;
            Invocations = invocations;
        }
    }

    /// <summary>
    /// Выполняет серию проверок и возвращает ValidationResult в случае успеха.
    /// </summary>
    /// <returns>Экземпляр ValidationResult с данными или null, если валидация не пройдена.</returns>
    public ValidationResult? Validate(TContext callerContext, IContextCollector<TContext> collector)
    {
        if(string.IsNullOrWhiteSpace(callerContext.FullName) || !collector.BySymbolDisplayName.TryGetValue(callerContext.FullName, out var callerContextInfo))
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"[MISS]: Symbol not found in {collector.BySymbolDisplayName} for {callerContext.FullName}");
            return null;
        }

        var callerSyntaxNode = callerContext.SyntaxNode;
        if(callerSyntaxNode == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Trace, $"[MISS]: SyntaxNode is not defined for {callerContext.FullName}");
            return null;
        }

        var canRaiseNoInvocationError = !(callerContext.ClassOwner?.ElementType == ContextInfoElementType.@class || callerContext.ClassOwner?.ElementType == ContextInfoElementType.@interface || callerContext.ClassOwner?.ElementType == ContextInfoElementType.@record);
        var invocationList = callerSyntaxNode.DescendantNodes<TInvocationExpressionSyntax>();
        if(!invocationList.Any() && canRaiseNoInvocationError)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"[MISS]: No invocation found for {callerContextInfo.GetDebugSymbolName()}");
        }

        return new ValidationResult(callerContextInfo, invocationList);
    }
}