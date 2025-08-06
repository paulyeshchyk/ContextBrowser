using ContextBrowser.ContextKit.Parser;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using Microsoft.CodeAnalysis;

namespace RoslynKit.Parser.Phases.SymbolLookupHandler;

/// <summary>
/// Обработчик, который пытается найти ContextInfo по полной сигнатуре метода,
/// если символ является IMethodSymbol.
/// </summary>
/// <typeparam name="TContext">Тип возвращаемого контекста.</typeparam>
public class MethodSymbolLookupHandler<TContext> : BaseSymbolLookupHandler<TContext>
    where TContext : ContextInfo, IContextWithReferences<TContext>
{
    private IContextCollector<TContext> _collector;

    /// <summary>
    /// Конструктор обработчика MethodSymbolLookupHandler.
    /// </summary>
    /// <param name="collector">Зависимость для сбора данных.</param>
    /// <param name="onWriteLog">Зависимость для логирования.</param>
    public MethodSymbolLookupHandler(IContextCollector<TContext> collector, OnWriteLog? onWriteLog)
        : base(onWriteLog)
    {
        _collector = collector;
    }

    /// <summary>
    /// Обрабатывает запрос, пытаясь найти ContextInfo по полной сигнатуре IMethodSymbol.
    /// </summary>
    /// <param name="symbolDto">Обертка над синтаксическим узлом вызова.</param>
    /// <returns>Найденный контекст или null, если не найден.</returns>
    public override TContext? Handle(InvocationSyntaxWrapper symbolDto)
    {
        if(symbolDto.Symbol is not IMethodSymbol methodSymbol)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"[MISS] Symbol is not IMethodSymbol: {symbolDto.FullName}");
            return base.Handle(symbolDto);
        }

        var fullName = methodSymbol.ToDisplayString();
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[FALLBACK] Trying full signature: {fullName}");

        if(!_collector.BySymbolDisplayName.TryGetValue(fullName, out var result))
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[MISS] Symbol exists but fallback lookup failed: {fullName}");

            // Если не применимо или не найдено, передаем запрос следующему обработчику.
            return base.Handle(symbolDto);
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[HIT] Recovered callee via full symbol: {fullName}");
        return result;
    }
}
