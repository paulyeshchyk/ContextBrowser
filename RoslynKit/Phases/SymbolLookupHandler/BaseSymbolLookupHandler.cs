using ContextBrowserKit.Log;
using ContextKit.Model;
using RoslynKit.Parser.Phases;

namespace RoslynKit.Phases.SymbolLookupHandler;

/// <summary>
/// Базовый абстрактный класс для обработчиков цепочки поиска символов.
/// </summary>
/// <typeparam name="TContext">Тип возвращаемого контекста.</typeparam>
public abstract class BaseSymbolLookupHandler<TContext> : ISymbolLookupHandler<TContext>
    where TContext : class, IContextWithReferences<TContext>
{
    protected ISymbolLookupHandler<TContext>? _nextHandler;
    protected readonly OnWriteLog? _onWriteLog; // Общая зависимость для логирования

    /// <summary>
    /// Конструктор базового обработчика.
    /// </summary>
    /// <param name="onWriteLog">Зависимость для логирования.</param>
    protected BaseSymbolLookupHandler(OnWriteLog? onWriteLog)
    {
        _onWriteLog = onWriteLog;
    }

    /// <summary>
    /// Устанавливает следующий обработчик в цепочке.
    /// </summary>
    /// <param name="handler">Следующий обработчик.</param>
    /// <returns>Следующий обработчик.</returns>
    public ISymbolLookupHandler<TContext> SetNext(ISymbolLookupHandler<TContext> handler)
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
    public virtual TContext? Handle(InvocationSyntaxWrapper symbolDto)
    {
        return _nextHandler?.Handle(symbolDto);
    }
}
