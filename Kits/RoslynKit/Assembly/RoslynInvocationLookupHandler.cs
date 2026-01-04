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
    where TContext : class, IContextWithReferences<TContext>
    where TSemanticModel : class, ISemanticModelWrapper

{
    private readonly SemanticOptions _options;
    private readonly IContextCollector<TContext> _collector;
    private readonly ContextInfoBuilderDispatcher<TContext> _contextInfoBuilderDispatcher;

    public RoslynInvocationLookupHandler(
        IContextCollector<TContext> collector,
        ContextInfoBuilderDispatcher<TContext> contextInfoBuilderDispatcher,
        IAppLogger<AppLevel> logger,
        SemanticOptions options)
        : base(logger)
    {
        _options = options;
        _collector = collector;
        _contextInfoBuilderDispatcher = contextInfoBuilderDispatcher;
    }

    // context: roslyn, syntax, build
    public override TContext? Handle(ISyntaxWrapper syntaxWrapper)
    {
        if (!_options.CreateFailedCallees)
        {
            // Если создание искусственных узлов не разрешено, возвращаем null

            _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Warn, $"[MISS] Fallback fake callee be not used for {syntaxWrapper.FullName}, because of disabled option CreateFailedCallees");
            return base.Handle(syntaxWrapper);
        }

        _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Dbg, $"[CREATE] Fallback fake callee created for: {syntaxWrapper.FullName}");

        // Логика создания искусственного узла

        var typeModel = new CSharpSyntaxWrapperType(syntaxWrapper: syntaxWrapper);
        var typeContext = _contextInfoBuilderDispatcher.DispatchAndBuild(null, typeModel);
        if (typeContext == null)
        {
            _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Err, $"[MISS] Fallback fake callee be not used for {syntaxWrapper.FullName}, because ContextInfoBuilder returns null");
            return base.Handle(syntaxWrapper);
        }

        var methodmodel = new CSharpSyntaxWrapperMethodArtifitial(wrapper: syntaxWrapper, contextOwner: typeContext);
        var methodContext = _contextInfoBuilderDispatcher.DispatchAndBuild(null, methodmodel);
        if (methodContext == null)
        {
            _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Err, $"[MISS] Fallback fake callee be not used for {syntaxWrapper.FullName}, because ContextInfoBuilder returns null for method");
            return base.Handle(syntaxWrapper);
        }

        _collector.Append(typeContext);

        typeContext.Owns.Add(methodContext);
        _collector.Append(methodContext);
        _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Dbg, $"[DONE] Fallback fake callee created for: {syntaxWrapper.FullName}");

        return methodContext;
    }
}
