using System.Threading;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Phases.Syntax.Parsers;

public abstract class BaseSyntaxParser<TContext> : ISyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    protected readonly IAppLogger<AppLevel> _logger;

    protected BaseSyntaxParser(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    public abstract bool CanParse(object syntax);

    public abstract void Parse(TContext? parent, object syntax, ISemanticModelWrapper model, SemanticOptions options, CancellationToken cancellationToken);
}