using System.Collections.Generic;
using System.Threading;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

public interface ISemanticSyntaxRouter<TContext>
{
    void Route(List<object> availableSyntaxies, ISemanticModelWrapper model, SemanticOptions options, CancellationToken cancellationToken);
}
