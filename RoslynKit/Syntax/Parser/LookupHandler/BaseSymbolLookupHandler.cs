using ContextBrowserKit.Log;
using ContextKit.Model;
using RoslynKit.Syntax.Parser.Wrappers;

namespace RoslynKit.Syntax.Parser.LookupHandler;

/// <summary>
/// Базовый абстрактный класс для обработчиков цепочки поиска символов.
/// </summary>
/// <typeparam name="TContext">Тип возвращаемого контекста.</typeparam>
public abstract class BaseSymbolLookupHandler<TContext, TSyntaxNode, TSemanticModel> : ISymbolLookupHandler<TContext, TSyntaxNode, TSemanticModel>
    where TContext : class, IContextWithReferences<TContext>
    where TSyntaxNode : class
    where TSemanticModel : class
{
    protected ISymbolLookupHandler<TContext, TSyntaxNode, TSemanticModel>? _nextHandler;
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
    public ISymbolLookupHandler<TContext, TSyntaxNode, TSemanticModel> SetNext(ISymbolLookupHandler<TContext, TSyntaxNode, TSemanticModel> handler)
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
    public virtual TContext? Handle(CSharpInvocationSyntaxWrapper symbolDto)
    {
        return _nextHandler?.Handle(symbolDto);
    }
}
