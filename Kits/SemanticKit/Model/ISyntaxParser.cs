using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

public interface ISyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    // Проверяет, может ли этот парсер обработать данный синтаксический узел.
    bool CanParseSyntax(object syntax);

    // Выполняет парсинг синтаксического узла.
    Task ParseAsync(TContext? parent, object syntax, ISemanticModelWrapper model, SemanticOptions options, CancellationToken cancellationToken);
}