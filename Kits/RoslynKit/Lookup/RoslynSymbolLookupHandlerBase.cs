using System.Threading;
using System.Threading.Tasks;
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
/// Базовый абстрактный класс для обработчиков цепочки поиска символов.
/// </summary>
/// <typeparam name="TContext">Тип возвращаемого контекста.</typeparam>
/// <typeparam name="TSemanticModel"></typeparam>
public abstract class RoslynSymbolLookupHandlerBase<TContext, TSemanticModel> : ISymbolLookupHandler<TContext, TSemanticModel>
    where TContext : IContextWithReferences<TContext>
    where TSemanticModel : class, ISemanticModelWrapper
{
    protected ISymbolLookupHandler<TContext, TSemanticModel>? _nextHandler;
    protected readonly IAppLogger<AppLevel> _logger; // Общая зависимость для логирования

    /// <summary>
    /// Конструктор базового обработчика.
    /// </summary>
    /// <param name="logger">Зависимость для логирования.</param>
    protected RoslynSymbolLookupHandlerBase(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Устанавливает следующий обработчик в цепочке.
    /// </summary>
    /// <param name="handler">Следующий обработчик.</param>
    /// <returns>Следующий обработчик.</returns>
    public ISymbolLookupHandler<TContext, TSemanticModel> SetNext(ISymbolLookupHandler<TContext, TSemanticModel> handler)
    {
        _nextHandler = handler;
        return handler;
    }

    /// <summary>
    /// Абстрактный метод для обработки запроса.
    /// Если обработчик не может обработать запрос, он передает его следующему в цепочке.
    /// </summary>
    /// <param name="symbolDto">Обертка над синтаксическим узлом вызова.</param>
    /// <returns>Найденный контекст или null.</returns>
    public virtual async Task<TContext?> HandleAsync(ISyntaxWrapper symbolDto, CancellationToken cancellationToken)
    {
        if (_nextHandler == null)
            return default;
        return await _nextHandler.HandleAsync(symbolDto, cancellationToken).ConfigureAwait(false);
    }
}
