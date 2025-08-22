using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using RoslynKit.Phases.ContextInfoBuilder;
using RoslynKit.Phases.Invocations.Lookup;
using RoslynKit.Wrappers.Syntax;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Wrappers.LookupHandler;

public class RoslynInvocationLookupHandler<TContext, TSemanticModel> : SymbolLookupHandler<TContext, TSemanticModel>
    where TContext : ContextInfo, IContextWithReferences<TContext>
    where TSemanticModel : class, ISemanticModelWrapper

{
    private readonly SemanticOptions _options;
    private IContextCollector<TContext> _collector;
    private readonly CSharpMethodContextInfoBuilder<TContext> _methodContextInfoBuilder;
    private readonly CSharpTypeContextInfoBulder<TContext> _typeContextInfoBuilder;

    public RoslynInvocationLookupHandler(IContextCollector<TContext> collector, OnWriteLog? onWriteLog, SemanticOptions options, CSharpTypeContextInfoBulder<TContext> typeContextInfoBuilder, CSharpMethodContextInfoBuilder<TContext> methodContextInfoBuilder) : base(onWriteLog)
    {
        _options = options;
        _typeContextInfoBuilder = typeContextInfoBuilder;
        _methodContextInfoBuilder = methodContextInfoBuilder;
        _collector = collector;
    }

    public override TContext? Handle(IInvocationSyntaxWrapper symbolDto)
    {
        if (!_options.CreateFailedCallees)
        {
            // Если создание искусственных узлов не разрешено, возвращаем null

            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"[MISS] Fallback fake callee be not used for {symbolDto.FullName}, because of disabled option CreateFailedCallees");
            return base.Handle(symbolDto);
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Trace, $"[CREATE] Fallback fake callee created for: {symbolDto.FullName}");

        // Логика создания искусственного узла

        var nameSpace = symbolDto.FullName.BeforeDot(1, 1).ToAlphanumericUnderscore();
        var className = symbolDto.FullName.BeforeDot(2, 1).ToAlphanumericUnderscore();
        var fullname = $"{nameSpace}.{className}";//symbolDto.FullName.BeforeDot(1, 2);

        var typeModel = new CSharpTypeSyntaxWrapper(name: className, fullName: fullname, symbolDto.SpanStart, symbolDto.SpanEnd, nameSpace);
        var typeContext = _typeContextInfoBuilder.BuildContextInfo(default, typeModel);
        if (typeContext == null)
        {
            return default;
        }

        _collector.Append(typeContext);

        var methodmodel = new CSharpMethodSyntaxWrapper(wrapper: symbolDto);
        var methodContext = _methodContextInfoBuilder.BuildInvocationContextInfo(typeContext, methodmodel);

        if (methodContext == null)
        {
            return default;
        }

        _collector.Append(methodContext);

        return methodContext;
    }
}