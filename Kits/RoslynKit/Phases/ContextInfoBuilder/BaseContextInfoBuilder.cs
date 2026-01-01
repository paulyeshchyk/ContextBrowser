using System.Threading;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using RoslynKit.Converters;
using SemanticKit.Model;
using SemanticKit.Model.SyntaxWrapper;

namespace RoslynKit.Phases.ContextInfoBuilder;

public interface IUniversalContextInfoBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
    ContextInfoElementType ElementType { get; }
    bool CanBuild(object syntax);
    bool CanBuild(ISyntaxWrapper syntax);
    TContext BuildContextInfo(TContext? ownerContext, object syntaxNode, ISemanticModelWrapper semanticModel, CancellationToken cancellationToken);
    TContext BuildContextInfo(TContext? ownerContext, IContextInfo dto);
}


public abstract class BaseContextInfoBuilder<TContext, TSyntaxNode, TSemanticModel, TWrapper> : IUniversalContextInfoBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
    where TSyntaxNode : class
    where TSemanticModel : ISemanticModelWrapper
    where TWrapper : ISyntaxNodeWrapper, new()
{
    protected readonly IContextCollector<TContext> _collector;
    protected readonly IContextFactory<TContext> _factory;
    protected readonly IAppLogger<AppLevel> _logger;

    protected BaseContextInfoBuilder(
        IContextCollector<TContext> collector,
        IContextFactory<TContext> factory,
        IAppLogger<AppLevel> logger)
    {
        _collector = collector;
        _factory = factory;
        _logger = logger;
    }

    public abstract ContextInfoElementType ElementType { get; }

    public bool CanBuild(object syntax) => syntax is TSyntaxNode;

    public abstract bool CanBuild(ISyntaxWrapper contextInfo);

    public virtual TContext BuildContextInfo(TContext? ownerContext, object syntaxNode, ISemanticModelWrapper semanticModel, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var syntaxNodeWrapper = new TWrapper();
        syntaxNodeWrapper.SetSyntax(syntaxNode);

        var symbolInfo = CSharpISymbolWrapperConverter.FromSymbolInfo(semanticModel, syntaxNodeWrapper, _logger, cancellationToken);
        var dto = ContextInfoDtoConverter.ConvertFromSyntaxNodeWrapper(ownerContext, syntaxNodeWrapper, symbolInfo, ElementType);

        return BuildContextInfo(ownerContext, dto);
    }

    public virtual TContext BuildContextInfo(TContext? ownerContext, IContextInfo dto)
    {
        _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Dbg, $"Creating method ContextInfo: {dto.Name}");

        var availableItem = _collector.GetItem(dto.FullName);
        if (availableItem != null)
            return availableItem;

        var result = _factory.Create(contextInfo: dto);

        ownerContext?.Owns.Add(result);

        _collector.Add(result);

        return result;
    }
}

