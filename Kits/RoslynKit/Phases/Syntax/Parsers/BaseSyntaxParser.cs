using ContextBrowserKit.Log;
using ContextKit.Model;
using SemanticKit.Model;

namespace RoslynKit.Phases.Syntax.Parsers;

public abstract class BaseSyntaxParser<TContext> : ISyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    protected readonly OnWriteLog? _onWriteLog;

    protected BaseSyntaxParser(OnWriteLog? onWriteLog)
    {
        _onWriteLog = onWriteLog;
    }

    public abstract bool CanParse(object syntax);

    public abstract void Parse(TContext? parent, object syntax, ISemanticModelWrapper model, CancellationToken cancellationToken);
}