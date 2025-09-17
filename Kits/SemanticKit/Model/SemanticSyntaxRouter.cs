using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Phases.Syntax;

public class SemanticSyntaxRouter<TContext> : ISemanticSyntaxRouter<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IEnumerable<ISyntaxParser<TContext>> _parsers;

    public SemanticSyntaxRouter(IAppLogger<AppLevel> logger, IEnumerable<ISyntaxParser<TContext>> parsers)
    {
        _logger = logger;
        _parsers = parsers;
    }

    public void Route(IEnumerable<object> availableSyntaxies, ISemanticModelWrapper model, SemanticOptions options, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, $"Routing syntaxies - ({availableSyntaxies.Count()})", LogLevelNode.Start);

        foreach (var item in availableSyntaxies)
        {
            var parser = _parsers.FirstOrDefault(p => p.CanParse(item));

            if (parser == null)
            {
                _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Err, $"[MISS]: No parser found for syntax type: {item}");
                continue;
            }

            parser.Parse(default, item, model, options, cancellationToken);
        }

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }
}
