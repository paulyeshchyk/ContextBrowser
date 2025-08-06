using ContextBrowser.ContextKit.Parser;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;

namespace RoslynKit.Parser.Phases.SymbolLookupHandler;

/// <summary>
/// Обработчик, который пытается найти ContextInfo по полному имени символа (symbolDto.FullName).
/// </summary>
/// <typeparam name="TContext">Тип возвращаемого контекста.</typeparam>
public class FullNameLookupHandler<TContext> : BaseSymbolLookupHandler<TContext>
    where TContext : ContextInfo, IContextWithReferences<TContext>
{
    private IContextCollector<TContext> _collector;


    /// <summary>
    /// Конструктор обработчика FullNameLookupHandler.
    /// </summary>
    /// <param name="collector">Зависимость для сбора данных.</param>
    /// <param name="onWriteLog">Зависимость для логирования.</param>
    public FullNameLookupHandler(IContextCollector<TContext> collector, OnWriteLog? onWriteLog)
        : base(onWriteLog)
    {
        _collector = collector;
    }

    /// <summary>
    /// Обрабатывает запрос, пытаясь найти ContextInfo по symbolDto.FullName.
    /// </summary>
    /// <param name="symbolDto">Обертка над синтаксическим узлом вызова.</param>
    /// <returns>Найденный контекст или null, если не найден.</returns>
    public override TContext? Handle(InvocationSyntaxWrapper symbolDto)
    {
        if(_collector.BySymbolDisplayName.TryGetValue(symbolDto.FullName, out var calleeContextInfo))
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[OK] Found ContextInfo by SymbolName: {symbolDto.FullName}");
            return calleeContextInfo as TContext; // Приведение к TContext
        }

        // Если не найдено, передаем запрос следующему обработчику в цепочке.
        return base.Handle(symbolDto);
    }
}
