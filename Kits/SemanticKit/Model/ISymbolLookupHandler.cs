using System.Threading.Tasks;
using ContextKit.Model;
using SemanticKit.Model.SyntaxWrapper;

namespace SemanticKit.Model;

/// <summary>
/// Определяет интерфейс для обработчиков в цепочке поиска символов.
/// </summary>
/// <typeparam name="TContext">Тип возвращаемого контекста.</typeparam>
/// <typeparam name="TSemanticModel"></typeparam>
public interface ISymbolLookupHandler<TContext, TSemanticModel>
    where TContext : IContextWithReferences<TContext>
    where TSemanticModel : class, ISemanticModelWrapper
{
    /// <summary>
    /// Устанавливает следующий обработчик в цепочке.
    /// </summary>
    /// <param name="handler">Следующий обработчик.</param>
    /// <returns>Следующий обработчик для удобства построения цепочки.</returns>
    ISymbolLookupHandler<TContext, TSemanticModel> SetNext(ISymbolLookupHandler<TContext, TSemanticModel> handler);

    /// <summary>
    /// Обрабатывает запрос на поиск контекстной информации для символа.
    /// </summary>
    /// <param name="symbolDto">Обертка над синтаксическим узлом вызова.</param>
    /// <returns>Найденный контекст или null, если обработчик не смог обработать запрос.</returns>
    Task<TContext?> Handle(ISyntaxWrapper symbolDto);
}
