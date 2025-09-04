using System;
using System.Threading;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using RoslynKit.Wrappers.Syntax;
using SemanticKit.Model;

namespace RoslynKit.Phases.ContextInfoBuilder;

public abstract class BaseContextInfoBuilder<TContext, TSyntaxNode, TSemanticModel, TWrapper>
    where TContext : IContextWithReferences<TContext>
    where TSyntaxNode : class
    where TSemanticModel : ISemanticModelWrapper
    where TWrapper : ISyntaxNodeWrapper, new()
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

    public abstract ContextInfoElementType ElementType { get; }

    public virtual TContext? BuildContextInfo(TContext? ownerContext, TSyntaxNode syntaxNode, TSemanticModel semanticModel, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var syntaxNodeWrapper = new TWrapper();
        syntaxNodeWrapper.SetSyntax(syntaxNode);

        var symbolInfo = CSharpISymbolWrapperConverter.FromSymbolInfo(semanticModel, syntaxNodeWrapper, _onWriteLog, cancellationToken);
        var dto = ContextInfoDtoConverter.ConvertFromSyntaxNodeWrapper(ownerContext, syntaxNodeWrapper, symbolInfo, ElementType);

        return BuildContextInfo(ownerContext, dto);
    }

    protected virtual TContext? BuildContextInfo(TContext? ownerContext, IContextInfo dto)
    {
        _onWriteLog?.Invoke(AppLevel.R_Cntx, LogLevel.Dbg, $"Creating method ContextInfo: {dto.Name}");

        var availableItem = _collector.GetItem(dto.FullName);
        if (availableItem != null)
            return availableItem;

        var result = _factory.Create(contextInfo: dto);

        if (result == null)
        {
            _onWriteLog?.Invoke(AppLevel.R_Cntx, LogLevel.Err, $"Creating method ContextInfo failed {dto.Name}");
            return default;
        }

        _collector.Add(result);

        return result;
    }
}
