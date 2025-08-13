using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using RoslynKit.Basics.Semantic;

namespace RoslynKit.Basics.ContextKit;

public class BaseContextInfoBuilder<TContext, TSyntaxNode, TSemanticModel>
    where TContext : IContextWithReferences<TContext>
    where TSyntaxNode : class
    where TSemanticModel : class
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

    protected virtual ISymInfoLoader BuildSymInfoDto(TContext? ownerContext, TSyntaxNode syntaxNode, TSemanticModel semanticModel)
    {
        throw new NotImplementedException();
    }

    public virtual TContext? BuildContextInfo(TContext? ownerContext, TSyntaxNode syntaxNode, TSemanticModel semanticModel)
    {
        var symInfo = BuildSymInfoDto(ownerContext, syntaxNode, semanticModel);
        return BuildContextInfo(ownerContext, symInfo);
    }

    public virtual TContext? BuildContextInfo(TContext? ownerContext, ISymInfoLoader symInfoLoader)
    {
        _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Dbg, $"Creating method ContextInfo: {symInfoLoader.Name}");

        var result = _factory.Create(
                  owner: ownerContext,
            elementType: symInfoLoader.Kind,
                 nsName: symInfoLoader.Namespace,
                   name: symInfoLoader.Name,
               fullName: symInfoLoader.FullName,
             syntaxNode: symInfoLoader.NodeInfo,
              spanStart: symInfoLoader.SpanStart,
                spanEnd: symInfoLoader.SpanEnd,
                 symbol: symInfoLoader.SymInfo);

        if(result == null)
        {
            _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Err, $"Creating method ContextInfo failed {symInfoLoader.Name}");
            return default;
        }

        _collector.Add(result);

        return result;
    }
}
