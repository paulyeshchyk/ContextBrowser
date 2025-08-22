namespace ContextBrowser.Model;

public interface IContextParser<TContext>
{
    Task<IEnumerable<TContext>> ParseAsync(string[] filePaths, CancellationToken ct);
}