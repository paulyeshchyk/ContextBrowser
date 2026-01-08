using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using RoslynKit;
using RoslynKit.Assembly;
using RoslynKit.Lookup;
using RoslynKit.Model.SyntaxWrapper;
using RoslynKit.Wrappers;
using SemanticKit.Model;
using SemanticKit.Model.Options;
using SemanticKit.Model.SyntaxWrapper;

namespace RoslynKit.Assembly;

// context: roslyn, syntax, build
public class RoslynInvocationLookupHandler<TContext, TSemanticModel> : SymbolLookupHandler<TContext, TSemanticModel>
    where TContext : IContextWithReferences<TContext>
    where TSemanticModel : class, ISemanticModelWrapper

{
    private readonly SemanticOptions _options;
    private readonly IContextCollector<TContext> _collector;
    private readonly ContextInfoBuilderDispatcher<TContext> _contextInfoBuilderDispatcher;
    private readonly ICSharpSyntaxWrapperTypeBuilder _syntaxWrapperTypeBuilder;
    private readonly object _lock = new();

    public RoslynInvocationLookupHandler(
        IContextCollector<TContext> collector,
        ContextInfoBuilderDispatcher<TContext> contextInfoBuilderDispatcher,
        IAppLogger<AppLevel> logger,
        SemanticOptions options,
        ICSharpSyntaxWrapperTypeBuilder syntaxWrapperTypeBuilder)
        : base(logger)
    {
        _options = options;
        _collector = collector;
        _contextInfoBuilderDispatcher = contextInfoBuilderDispatcher;
        _syntaxWrapperTypeBuilder = syntaxWrapperTypeBuilder;

    }

    // context: roslyn, syntax, build
    public override async Task<TContext?> Handle(ISyntaxWrapper syntaxWrapper)
    {
        if (!_options.CreateFailedCallees)
        {
            // Если создание искусственных узлов не разрешено, возвращаем null

            _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Warn, $"[MISS] Fallback fake callee be not used for {syntaxWrapper.FullName}, because of disabled option CreateFailedCallees");
            return await base.Handle(syntaxWrapper);
        }

        _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Dbg, $"[CREATE] Fallback fake callee created for: {syntaxWrapper.FullName}");

        // Логика создания искусственного узла
        var typeModel = _syntaxWrapperTypeBuilder.BuildType(syntaxWrapper);

        var typeContext = await _contextInfoBuilderDispatcher.DispatchAndBuild(default, typeModel).ConfigureAwait(false);
        if (typeContext == null)
        {
            _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Err, $"[MISS] Fallback fake callee be not used for {syntaxWrapper.FullName}, because ContextInfoBuilder returns null");
            return await base.Handle(syntaxWrapper);
        }
        var methodmodel = new CSharpSyntaxWrapperMethodArtifitial(wrapper: syntaxWrapper, contextOwner: typeContext);
        var methodContext = await _contextInfoBuilderDispatcher.DispatchAndBuild(default, methodmodel).ConfigureAwait(false);
        if (methodContext == null)
        {
            _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Err, $"[MISS] Fallback fake callee be not used for {syntaxWrapper.FullName}, because ContextInfoBuilder returns null for method");
            return await base.Handle(syntaxWrapper);
        }

        _collector.Append(item: typeContext, owns: methodContext);
        _collector.Append(methodContext);
        _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Dbg, $"[DONE] Fallback fake callee created for: {syntaxWrapper.FullName}");
        return methodContext;
    }
}
