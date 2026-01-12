using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;
using SemanticKit.Model.SyntaxWrapper;

namespace SemanticKit.Model;

/// <summary>
/// Определяет последовательность(номер очереди) вызовов обработчиков
/// </summary>
public interface ISymbolLookupHandlerChainFactory<TContext, TModel>
    where TContext : IContextWithReferences<TContext>
    where TModel : class, ISemanticModelWrapper
{
    // Метод, который возвращает первый элемент собранной цепочки
    ISymbolLookupHandler<TContext, TModel> BuildChain();
}