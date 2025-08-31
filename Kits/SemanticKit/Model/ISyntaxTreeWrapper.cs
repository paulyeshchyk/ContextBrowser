using SemanticKit.Model.Options;

namespace SemanticKit.Model;

public interface ISyntaxTreeWrapper
{
    object GetRoot();

    string FilePath { get; }

    object GetCompilationUnitRoot(CancellationToken cancellationToken);

    IEnumerable<object> GetAvailableSyntaxies(SemanticOptions options, CancellationToken cancellationToken);

    object Tree { get; }
}
