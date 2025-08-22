using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using SemanticKit.Model;

namespace RoslynKit.Phases.Syntax;

public class SemanticSyntaxRouter<TContext> : ISemanticSyntaxRouter<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly OnWriteLog? _onWriteLog;
    private readonly IEnumerable<ISyntaxParser<TContext>> _parsers;

    public SemanticSyntaxRouter(OnWriteLog? onWriteLog, IEnumerable<ISyntaxParser<TContext>> parsers)
    {
        _onWriteLog = onWriteLog;
        _parsers = parsers;
    }

    public void Route(IEnumerable<object> availableSyntaxies, ISemanticModelWrapper model, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Routing syntaxies - ({availableSyntaxies.Count()})", LogLevelNode.Start);

        foreach (var item in availableSyntaxies)
        {
            var parser = _parsers.FirstOrDefault(p => p.CanParse(item));

            if (parser == null)
            {
                _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"[MISS]: No parser found for syntax type: {item}");
                continue;
            }

            parser.Parse(default, item, model, cancellationToken);
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }
}
