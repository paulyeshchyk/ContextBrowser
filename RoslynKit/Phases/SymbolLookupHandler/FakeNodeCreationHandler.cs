using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using RoslynKit.Model;
using RoslynKit.Model.Wrappers;
using RoslynKit.Parser.Phases;
using RoslynKit.Semantic.Builder;

namespace RoslynKit.Phases.SymbolLookupHandler;

public class FakeNodeCreationHandler<TContext> : BaseSymbolLookupHandler<TContext>
    where TContext : ContextInfo, IContextWithReferences<TContext>
{
    private readonly RoslynCodeParserOptions _options;
    private IContextCollector<TContext> _collector;
    private readonly MethodContextInfoBuilder<TContext> _methodContextInfoBuilder;
    private readonly TypeContextInfoBulder<TContext> _typeContextInfoBuilder;

    public FakeNodeCreationHandler(IContextCollector<TContext> collector, OnWriteLog? onWriteLog, RoslynCodeParserOptions options, TypeContextInfoBulder<TContext> typeContextInfoBuilder, MethodContextInfoBuilder<TContext> methodContextInfoBuilder) : base(onWriteLog)
    {
        _options = options;
        _typeContextInfoBuilder = typeContextInfoBuilder;
        _methodContextInfoBuilder = methodContextInfoBuilder;
        _collector = collector;
    }

    public override TContext? Handle(InvocationSyntaxWrapper symbolDto)
    {
        if(!_options.CreateFailedCallees)
        {
            // Если создание искусственных узлов не разрешено, возвращаем null

            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"[MISS] Fallback fake callee be not used for {symbolDto.FullName}, because of disabled option CreateFailedCallees");
            return base.Handle(symbolDto);
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Trace, $"[CREATE] Fallback fake callee created for: {symbolDto.FullName}");

        // Логика создания искусственного узла
        var className = symbolDto.FullName.BeforeFirstDot().ToAlphanumericUnderscore();
        var ns = _options.ExternalNamespaceName;
        var fullname = string.Join(".", new string[] { ns, className });

        var typeModel = new TypeSyntaxWrapper(kind: ContextInfoElementType.@class, typeFullName: fullname, symbolName: className);
        var typeContext = _typeContextInfoBuilder.BuildContextInfoForType(typeModel, ns);
        if(typeContext == null)
        {
            return default;
        }

        _collector.Append(typeContext);

        var methodmodel = new MethodSyntaxWrapper(symbol: symbolDto.Symbol, symbolDto.ShortName, symbolDto.SpanStart, symbolDto.SpanEnd);
        var methodContext = _methodContextInfoBuilder.BuildContextInfoForMethod(typeContext, methodmodel, _options.ExternalNamespaceName, null);

        if(methodContext == null)
        {
            return default;
        }

        _collector.Append(methodContext);

        return methodContext;
    }
}