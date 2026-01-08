using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

public abstract class SyntaxParser<TContext> : ISyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    protected readonly IAppLogger<AppLevel> _logger;

    protected SyntaxParser(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    public abstract bool CanParseSyntax(object syntax);

    public abstract Task ParseAsync(TContext? parent, object syntax, ISemanticModelWrapper model, SemanticOptions options, CancellationToken cancellationToken);
}