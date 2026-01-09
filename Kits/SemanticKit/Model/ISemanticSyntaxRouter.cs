using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

public interface ISemanticSyntaxRouter<TContext>
{
    Task RouteAsync(List<object> availableSyntaxies, ISemanticModelWrapper model, SemanticOptions options, CancellationToken cancellationToken);
}
