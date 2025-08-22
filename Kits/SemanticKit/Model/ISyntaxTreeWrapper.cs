using SemanticKit.Model.Options;

namespace SemanticKit.Model;

public interface ISyntaxTreeWrapper
{
    // Возвращает корневой узел синтаксического дерева.
    object GetRoot();

    // Возвращает путь к файлу, связанный с деревом.
    string FilePath { get; }

    object GetCompilationUnitRoot(CancellationToken cancellationToken);

    IEnumerable<object> GetAvailableSyntaxies(SemanticOptions options, CancellationToken cancellationToken);

    object Tree { get; }
}
