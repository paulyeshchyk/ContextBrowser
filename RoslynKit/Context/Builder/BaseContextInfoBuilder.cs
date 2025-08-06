using ContextBrowser.ContextKit.Parser;
using ContextKit.Model;
using LoggerKit;

namespace RoslynKit.Context.Builder;

public abstract class BaseContextInfoBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
    protected readonly IContextCollector<TContext> _collector;
    protected readonly IContextFactory<TContext> _factory;
    protected readonly OnWriteLog? _onWriteLog;

    protected BaseContextInfoBuilder(
        IContextCollector<TContext> collector,
        IContextFactory<TContext> factory,
        OnWriteLog? onWriteLog)
    {
        _collector = collector;
        _factory = factory;
        _onWriteLog = onWriteLog;
    }
}
