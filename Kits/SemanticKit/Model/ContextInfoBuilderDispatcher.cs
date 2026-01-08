using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using SemanticKit.Model.SyntaxWrapper;

namespace SemanticKit.Model;

// context: ContextInfo, build
public class ContextInfoBuilderDispatcher<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly IEnumerable<IContextInfoBuilder<TContext>> _builders;

    private readonly IAppLogger<AppLevel> _logger;

    public ContextInfoBuilderDispatcher(
        IEnumerable<IContextInfoBuilder<TContext>> builders,
        IAppLogger<AppLevel> logger)
    {
        _builders = builders;
        _logger = logger;
    }

    // context: ContextInfo, build
    public async Task<TContext?> DispatchAndBuildAsync(TContext? parent, object syntax, ISemanticModelWrapper semanticModelWrapper, CancellationToken cancellationToken)
    {
        var builder = _builders.FirstOrDefault(b => b.CanBuild(syntax));

        if (builder == null)
        {
            _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Err, $"No builder found for syntax type: {syntax.GetType().Name}");
            return default;
        }

        // Вызываем универсальный метод, который вызывает специфичный BuildContextInfo внутри
        return await builder.BuildContextInfo(parent, syntax, semanticModelWrapper, cancellationToken).ConfigureAwait(false);
    }

    public async Task<TContext?> DispatchAndBuild(TContext? ownerContext, ISyntaxWrapper syntaxWrapper)
    {
        var builder = _builders.FirstOrDefault(b => b.CanBuild(syntaxWrapper));

        if (builder == null)
        {
            _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Err, $"No builder found for wrapper: {syntaxWrapper}");
            return default;
        }

        return await builder.BuildContextInfo(ownerContext, syntaxWrapper.GetContextInfoDto()).ConfigureAwait(false);
    }
}