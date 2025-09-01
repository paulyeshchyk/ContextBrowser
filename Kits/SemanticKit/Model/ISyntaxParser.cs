using System.Threading;
using ContextKit.Model;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

public interface ISyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    // Проверяет, может ли этот парсер обработать данный синтаксический узел.
    bool CanParse(object syntax);

    // Выполняет парсинг синтаксического узла.
    void Parse(TContext? parent, object syntax, ISemanticModelWrapper model, SemanticOptions options, CancellationToken cancellationToken);
}