using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using SemanticKit.Model;

namespace RoslynKit.Route.Phases.ContextInfoBuilder;

public class BaseContextInfoBuilder<TContext, TSyntaxNode, TSemanticModel>
    where TContext : IContextWithReferences<TContext>
    where TSyntaxNode : class
    where TSemanticModel : ISemanticModelWrapper
{
    protected readonly IContextCollector<TContext> _collector;
    protected readonly IContextFactory<TContext> _factory;
    protected readonly OnWriteLog? _onWriteLog;

    public BaseContextInfoBuilder(
        IContextCollector<TContext> collector,
        IContextFactory<TContext> factory,
        OnWriteLog? onWriteLog)
    {
        _collector = collector;
        _factory = factory;
        _onWriteLog = onWriteLog;
    }

    protected virtual IContextInfo BuildContextInfoDto(TContext? ownerContext, TSyntaxNode syntaxNode, TSemanticModel semanticModel, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual TContext? BuildContextInfo(TContext? ownerContext, TSyntaxNode syntaxNode, TSemanticModel semanticModel, CancellationToken cancellationToken)
    {
        var dto = BuildContextInfoDto(ownerContext, syntaxNode, semanticModel, cancellationToken);
        return BuildContextInfo(ownerContext, dto);
    }

    public virtual TContext? BuildContextInfo(TContext? ownerContext, IContextInfo dto)
    {
        _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Dbg, $"Creating method ContextInfo: {dto.Name}");

        var result = _factory.Create(contextInfo: dto);

        if(result == null)
        {
            _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Err, $"Creating method ContextInfo failed {dto.Name}");
            return default;
        }

        _collector.Add(result);

        return result;
    }
}
