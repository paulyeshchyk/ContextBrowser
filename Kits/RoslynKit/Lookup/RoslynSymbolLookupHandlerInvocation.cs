using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using RoslynKit;
using RoslynKit.Assembly;
using RoslynKit.Assembly.Strategy;
using RoslynKit.Assembly.Strategy.Invocation;
using RoslynKit.Lookup;
using RoslynKit.Model.SyntaxWrapper;
using SemanticKit.Model;
using SemanticKit.Model.Options;
using SemanticKit.Model.Signature;
using SemanticKit.Model.SyntaxWrapper;

namespace RoslynKit.Lookup;

// context: roslyn, syntax, build
public class RoslynSymbolLookupHandlerInvocation<TContext, TSemanticModel> : RoslynSymbolLookupHandlerBase<TContext, TSemanticModel>
    where TContext : IContextWithReferences<TContext>
    where TSemanticModel : class, ISemanticModelWrapper

{
    private readonly IContextCollector<TContext> _collector;
    private readonly ContextInfoBuilderDispatcher<TContext> _contextInfoBuilderDispatcher;
    private readonly ICSharpSyntaxWrapperTypeBuilderDefault _syntaxWrapperTypeBuilderDefault;
    private readonly ICSharpSyntaxWrapperTypeBuilderSigned _syntaxWrapperTypeBuilderSigned;
    private readonly IAppOptionsStore _optionsStore;

    private readonly object _lock = new();

    public RoslynSymbolLookupHandlerInvocation(
        IContextCollector<TContext> collector,
        ContextInfoBuilderDispatcher<TContext> contextInfoBuilderDispatcher,
        IAppLogger<AppLevel> logger,
        IAppOptionsStore optionsStore,
        ICSharpSyntaxWrapperTypeBuilderDefault syntaxWrapperTypeBuilder,
        ICSharpSyntaxWrapperTypeBuilderSigned syntaxWrapperTypeBuilderSigned)
        : base(logger)
    {
        _optionsStore = optionsStore;
        _collector = collector;
        _contextInfoBuilderDispatcher = contextInfoBuilderDispatcher;
        _syntaxWrapperTypeBuilderDefault = syntaxWrapperTypeBuilder;
        _syntaxWrapperTypeBuilderSigned = syntaxWrapperTypeBuilderSigned;
    }

    // context: roslyn, syntax, build
    public override async Task<TContext?> HandleAsync(ISyntaxWrapper syntaxWrapper, CancellationToken cancellationToken)
    {
        var options = _optionsStore.GetOptions<CodeParsingOptions>().SemanticOptions;
        if (!options.CreateFailedCallees)
        {
            // Если создание искусственных узлов не разрешено, возвращаем null

            _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Warn, $"[MISS] Fallback fake callee be not used for {syntaxWrapper.FullName}, because of disabled option CreateFailedCallees");
            return await base.HandleAsync(syntaxWrapper, cancellationToken).ConfigureAwait(false);
        }

        _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Dbg, $"[CREATE] Fallback fake callee created for: {syntaxWrapper.FullName}");

        // Логика создания искусственного узла
        var typeModel = syntaxWrapper.Signature is ISignature signature
            ? _syntaxWrapperTypeBuilderSigned.BuildSignedWrapper(syntaxWrapper, signature)
            : _syntaxWrapperTypeBuilderDefault.BuildDefaultWrapper(syntaxWrapper);

        var typeContext = await _contextInfoBuilderDispatcher.DispatchAndBuildAsync(default, typeModel, cancellationToken).ConfigureAwait(false);
        if (typeContext == null)
        {
            _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Err, $"[MISS] Fallback fake callee be not used for {syntaxWrapper.FullName}, because ContextInfoBuilder returns null");
            return await base.HandleAsync(syntaxWrapper, cancellationToken);
        }
        var methodmodel = new CSharpSyntaxWrapperMethodArtifitial(wrapper: syntaxWrapper, contextOwner: typeContext);
        var methodContext = await _contextInfoBuilderDispatcher.DispatchAndBuildAsync(default, methodmodel, cancellationToken).ConfigureAwait(false);
        if (methodContext == null)
        {
            _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Err, $"[MISS] Fallback fake callee be not used for {syntaxWrapper.FullName}, because ContextInfoBuilder returns null for method");
            return await base.HandleAsync(syntaxWrapper, cancellationToken);
        }

        _collector.Append(item: typeContext, owns: methodContext);
        _collector.Append(methodContext);
        _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Dbg, $"[DONE] Fallback fake callee created for: {syntaxWrapper.FullName}");
        return methodContext;
    }
}
