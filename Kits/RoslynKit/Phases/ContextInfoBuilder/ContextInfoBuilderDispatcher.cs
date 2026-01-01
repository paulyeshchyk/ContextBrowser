using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using SemanticKit.Model;
using SemanticKit.Model.SyntaxWrapper;

namespace RoslynKit.Phases.ContextInfoBuilder;

public class ContextInfoBuilderDispatcher<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly IEnumerable<IUniversalContextInfoBuilder<TContext>> _builders;

    private readonly IAppLogger<AppLevel> _logger;

    public ContextInfoBuilderDispatcher(
        IEnumerable<IUniversalContextInfoBuilder<TContext>> builders,
        IAppLogger<AppLevel> logger)
    {
        _builders = builders;
        _logger = logger;
    }

    public TContext? DispatchAndBuild(TContext? parent, object syntax, ISemanticModelWrapper semanticModelWrapper, CancellationToken cancellationToken)
    {
        var builder = _builders.FirstOrDefault(b => b.CanBuild(syntax));

        if (builder == null)
        {
            _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Err, $"No builder found for syntax type: {syntax.GetType().Name}");
            return default;
        }

        // Вызываем универсальный метод, который вызывает специфичный BuildContextInfo внутри
        return builder.BuildContextInfo(parent, syntax, semanticModelWrapper, cancellationToken);
    }

    public TContext? DispatchAndBuild(TContext? ownerContext, ISyntaxWrapper syntaxWrapper)
    {
        var builder = _builders.FirstOrDefault(b => b.CanBuild(syntaxWrapper));

        if (builder == null)
        {
            _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Err, $"No builder found for wrapper: {syntaxWrapper}");
            return default;
        }

        // Вызываем универсальный метод, который вызывает специфичный BuildContextInfo внутри
        return builder.BuildContextInfo(ownerContext, syntaxWrapper.GetContextInfoDto());
    }

}

