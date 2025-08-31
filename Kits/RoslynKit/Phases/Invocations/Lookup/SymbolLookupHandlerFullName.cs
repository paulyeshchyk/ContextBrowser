using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using SemanticKit.Model;

namespace RoslynKit.Phases.Invocations.Lookup;

/// <summary>
/// Обработчик, который пытается найти ContextInfo по полному имени символа (symbolDto.FullName).
/// </summary>
/// <typeparam name="TContext">Тип возвращаемого контекста.</typeparam>
public class SymbolLookupHandlerFullName<TContext, TSemanticModel> : SymbolLookupHandler<TContext, TSemanticModel>
    where TContext : ContextInfo, IContextWithReferences<TContext>
    where TSemanticModel : class, ISemanticModelWrapper

{
    private readonly IContextCollector<TContext> _collector;

    /// <summary>
    /// Конструктор обработчика FullNameLookupHandler.
    /// </summary>
    /// <param name="collector">Зависимость для сбора данных.</param>
    /// <param name="onWriteLog">Зависимость для логирования.</param>
    public SymbolLookupHandlerFullName(IContextCollector<TContext> collector, OnWriteLog? onWriteLog)
        : base(onWriteLog)
    {
        _collector = collector;
    }

    /// <summary>
    /// Обрабатывает запрос, пытаясь найти ContextInfo по symbolDto.FullName.
    /// </summary>
    /// <param name="symbolDto">Обертка над синтаксическим узлом вызова.</param>
    /// <returns>Найденный контекст или null, если не найден.</returns>
    public override TContext? Handle(ISyntaxWrapper symbolDto)
    {
        if (_collector.BySymbolDisplayName.TryGetValue(symbolDto.FullName, out var calleeContextInfo))
        {
            _onWriteLog?.Invoke(AppLevel.R_Cntx, LogLevel.Dbg, $"[OK  ] ContextInfo was found for {symbolDto.FullName}");
            return calleeContextInfo;
        }


        _onWriteLog?.Invoke(AppLevel.R_Cntx, LogLevel.Trace, $"[MISS] ContextInfo not found for {symbolDto.FullName}");

        // Если не найдено, передаем запрос следующему обработчику в цепочке.
        return base.Handle(symbolDto);
    }
}
