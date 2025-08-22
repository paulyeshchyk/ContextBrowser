using ContextBrowserKit.Log;
using ContextKit.Model;
using SemanticKit.Model;

namespace RoslynKit.Phases.Invocations.Lookup;

/// <summary>
/// Базовый абстрактный класс для обработчиков цепочки поиска символов.
/// </summary>
/// <typeparam name="TContext">Тип возвращаемого контекста.</typeparam>
public abstract class SymbolLookupHandler<TContext, TSemanticModel> : ISymbolLookupHandler<TContext, TSemanticModel>
    where TContext : class, IContextWithReferences<TContext>
    where TSemanticModel : class, ISemanticModelWrapper
{
    protected ISymbolLookupHandler<TContext, TSemanticModel>? _nextHandler;
    protected readonly OnWriteLog? _onWriteLog; // Общая зависимость для логирования

    /// <summary>
    /// Конструктор базового обработчика.
    /// </summary>
    /// <param name="onWriteLog">Зависимость для логирования.</param>
    protected SymbolLookupHandler(OnWriteLog? onWriteLog)
    {
        _onWriteLog = onWriteLog;
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
    public virtual TContext? Handle(IInvocationSyntaxWrapper symbolDto)
    {
        return _nextHandler?.Handle(symbolDto);
    }
}
