using SemanticKit.Model.Options;

namespace ContextBrowser.Model;

public interface IContextParser<TContext>
{
    Task<IEnumerable<TContext>> ParseAsync(string[] filePaths, SemanticOptions options, CancellationToken ct);
}