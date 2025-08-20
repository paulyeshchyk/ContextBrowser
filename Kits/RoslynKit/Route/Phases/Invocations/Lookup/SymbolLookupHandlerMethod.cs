using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using SemanticKit.Model;

namespace RoslynKit.Route.Phases.Invocations.Lookup;

/// <summary>
/// Обработчик, который пытается найти ContextInfo по полной сигнатуре метода,
/// если символ является IMethodSymbol.
/// </summary>
/// <typeparam name="TContext">Тип возвращаемого контекста.</typeparam>
public class SymbolLookupHandlerMethod<TContext, TSemanticModel> : SymbolLookupHandler<TContext, TSemanticModel>
    where TContext : ContextInfo, IContextWithReferences<TContext>
    where TSemanticModel : class, ISemanticModelWrapper

{
    private IContextCollector<TContext> _collector;

    /// <summary>
    /// Конструктор обработчика MethodSymbolLookupHandler.
    /// </summary>
    /// <param name="collector">Зависимость для сбора данных.</param>
    /// <param name="onWriteLog">Зависимость для логирования.</param>
    public SymbolLookupHandlerMethod(IContextCollector<TContext> collector, OnWriteLog? onWriteLog)
        : base(onWriteLog)
    {
        _collector = collector;
    }

    /// <summary>
    /// Обрабатывает запрос, пытаясь найти ContextInfo по полной сигнатуре IMethodSymbol.
    /// </summary>
    /// <param name="symbolDto">Обертка над синтаксическим узлом вызова.</param>
    /// <returns>Найденный контекст или null, если не найден.</returns>
    public override TContext? Handle(IInvocationSyntaxWrapper symbolDto)
    {
        if(!symbolDto.IsValid || symbolDto.ToDisplayString() is not string fullName)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Trace, $"[MISS] Symbol is not IMethodSymbol: {symbolDto.FullName}");
            return base.Handle(symbolDto);
        }


        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Trace, $"[FALLBACK] Trying full signature: {fullName}");

        if(!_collector.BySymbolDisplayName.TryGetValue(fullName, out var result))
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Trace, $"[MISS] Symbol exists but fallback lookup failed: {fullName}");

            // Если не применимо или не найдено, передаем запрос следующему обработчику.
            return base.Handle(symbolDto);
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Trace, $"[HIT] Recovered callee via full symbol: {fullName}");
        return result;
    }
}
