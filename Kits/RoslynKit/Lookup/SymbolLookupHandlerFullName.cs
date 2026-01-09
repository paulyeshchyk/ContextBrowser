using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using RoslynKit;
using RoslynKit.Lookup;
using RoslynKit.Phases;
using SemanticKit.Model;
using SemanticKit.Model.SyntaxWrapper;

namespace RoslynKit.Lookup;

/// <summary>
/// Обработчик, который пытается найти ContextInfo по полному имени символа (symbolDto.FullName).
/// </summary>
/// <typeparam name="TContext">Тип возвращаемого контекста.</typeparam>
/// <typeparam name="TSemanticModel"></typeparam>
public class SymbolLookupHandlerFullName<TContext, TSemanticModel> : SymbolLookupHandler<TContext, TSemanticModel>
    where TContext : IContextWithReferences<TContext>
    where TSemanticModel : class, ISemanticModelWrapper

{
    private readonly IContextCollector<TContext> _collector;

    /// <summary>
    /// Конструктор обработчика FullNameLookupHandler.
    /// </summary>
    /// <param name="collector">Зависимость для сбора данных.</param>
    /// <param name="logger">Зависимость для логирования.</param>
    public SymbolLookupHandlerFullName(IContextCollector<TContext> collector, IAppLogger<AppLevel> logger)
        : base(logger)
    {
        _collector = collector;
    }

    /// <summary>
    /// Обрабатывает запрос, пытаясь найти ContextInfo по symbolDto.FullName.
    /// </summary>
    /// <param name="symbolDto">Обертка над синтаксическим узлом вызова.</param>
    /// <returns>Найденный контекст или null, если не найден.</returns>
    public override async Task<TContext?> Handle(ISyntaxWrapper symbolDto)
    {
        if (_collector.Collection.TryGetValue(symbolDto.FullName, out var calleeContextInfo))
        {
            _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Dbg, $"[OK  ] ContextInfo was found for {symbolDto.FullName}");
            return calleeContextInfo;
        }

        _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Trace, $"[MISS] ContextInfo not found for {symbolDto.FullName}");

        // Если не найдено, передаем запрос следующему обработчику в цепочке.
        return await base.Handle(symbolDto).ConfigureAwait(false);
    }
}
