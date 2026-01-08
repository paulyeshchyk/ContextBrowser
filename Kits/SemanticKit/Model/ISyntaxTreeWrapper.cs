using System.Collections.Generic;
using System.Threading;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

public interface ISyntaxTreeWrapper
{
    string FilePath { get; }

    object Tree { get; }

    object GetCompilationUnitRoot(CancellationToken cancellationToken);

    IEnumerable<object> GetAvailableSyntaxies(SemanticOptions options, CancellationToken cancellationToken);
}
