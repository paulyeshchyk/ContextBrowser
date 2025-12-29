using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using RoslynKit.Phases.ContextInfoBuilder;
using RoslynKit.Phases.Invocations.Lookup;
using RoslynKit.Wrappers.Syntax;
using SemanticKit.Model;
using SemanticKit.Model.Options;
using SemanticKit.Model.SyntaxWrapper;

namespace RoslynKit.Wrappers.LookupHandler;

public class RoslynInvocationLookupHandler<TContext, TSemanticModel> : SymbolLookupHandler<TContext, TSemanticModel>
    where TContext : ContextInfo, IContextWithReferences<TContext>
    where TSemanticModel : class, ISemanticModelWrapper

{
    private readonly SemanticOptions _options;
    private readonly IContextCollector<TContext> _collector;
    private readonly CSharpMethodContextInfoBuilder<TContext> _methodContextInfoBuilder;
    private readonly CSharpTypeContextInfoBulder<TContext> _typeContextInfoBuilder;

    public RoslynInvocationLookupHandler(IContextCollector<TContext> collector, IAppLogger<AppLevel> logger, SemanticOptions options, CSharpTypeContextInfoBulder<TContext> typeContextInfoBuilder, CSharpMethodContextInfoBuilder<TContext> methodContextInfoBuilder)
        : base(logger)
    {
        _options = options;
        _typeContextInfoBuilder = typeContextInfoBuilder;
        _methodContextInfoBuilder = methodContextInfoBuilder;
        _collector = collector;
    }

    public override TContext? Handle(ISyntaxWrapper symbolDto)
    {
        if (!_options.CreateFailedCallees)
        {
            // Если создание искусственных узлов не разрешено, возвращаем null

            _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Warn, $"[MISS] Fallback fake callee be not used for {symbolDto.FullName}, because of disabled option CreateFailedCallees");
            return base.Handle(symbolDto);
        }

        _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Dbg, $"[CREATE] Fallback fake callee created for: {symbolDto.FullName}");

        // Логика создания искусственного узла

        var typeModel = new CSharpTypeSyntaxWrapper(syntaxWrapper: symbolDto, spanStart: symbolDto.SpanStart, spanEnd: symbolDto.SpanEnd);
        var typeContext = _typeContextInfoBuilder.BuildContextInfo(ownerContext: default, typeModel);
        if (typeContext == null)
        {
            _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Err, $"[FAIL] Typecontext not found for: {symbolDto.FullName}");
            return default;
        }

        _collector.Append(typeContext);

        var methodmodel = new CSharpMethodSyntaxWrapper(wrapper: symbolDto);
        var methodContext = _methodContextInfoBuilder.BuildInvocationContextInfo(ownerContext: typeContext, methodmodel);

        if (methodContext == null)
        {
            _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Err, $"[FAIL] Methodcontext not found for: {symbolDto.FullName}");
            return default;
        }

        _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Dbg, $"[DONE] Fallback fake callee created for: {symbolDto.FullName}");

        typeContext.Owns.Add(methodContext);
        _collector.Append(methodContext);

        return methodContext;
    }
}
