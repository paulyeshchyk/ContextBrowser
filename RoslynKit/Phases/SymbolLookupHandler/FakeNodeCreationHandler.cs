using ContextBrowser.ContextKit.Parser;
using ContextKit.Extensions;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using RoslynKit.Context.Builder;
using RoslynKit.Model;
using RoslynKit.Model.Wrappers;
using RoslynKit.Parser.Phases;

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

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[CREATE] Fallback fake callee created for: {symbolDto.FullName}");

        // Логика создания искусственного узла
        var className = symbolDto.FullName.BeforeFirstDot().ToAlphanumericUnderscore();
        var ns = _options.FakeNamespaceName;
        var fullname = string.Join(".", new string[] { ns, className });

        var typeModel = new TypeSyntaxWrapper(kind: ContextInfoElementType.@class, typeFullName: fullname, symbolName: className);
        var typeContext = _typeContextInfoBuilder.BuildContextInfoForType(typeModel, ns);

        _collector.Append(typeContext);

        var methodmodel = new MethodSyntaxWrapper(symbol: symbolDto.Symbol, symbolDto.FullName, symbolDto.SpanStart, symbolDto.SpanEnd);
        var methodContext = _methodContextInfoBuilder.BuildContextInfoForMethod(typeContext, methodmodel, _options.FakeNamespaceName, null);

        _collector.Append(methodContext);


        return methodContext;
    }
}