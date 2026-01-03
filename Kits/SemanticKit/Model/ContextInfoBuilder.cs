using System.Threading;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using SemanticKit.Model.SyntaxWrapper;

namespace SemanticKit.Model;

// context: ContextInfo, build
public interface IContextInfoBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
    ContextInfoElementType ElementType { get; }
    bool CanBuild(object syntax);
    bool CanBuild(ISyntaxWrapper syntax);

    // context: ContextInfo, build
    TContext BuildContextInfo(TContext? ownerContext, object syntaxNode, ISemanticModelWrapper semanticModel, CancellationToken cancellationToken);

    // context: ContextInfo, build
    TContext BuildContextInfo(TContext? ownerContext, IContextInfo dto);
}

// context: ContextInfo, build
public abstract class ContextInfoBuilder<TContext, TSyntaxNode, TWrapper> : IContextInfoBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
    where TSyntaxNode : class
    where TWrapper : ISyntaxNodeWrapper, new()
{
    protected readonly IContextCollector<TContext> _collector;
    protected readonly IContextFactory<TContext> _factory;
    protected readonly IAppLogger<AppLevel> _logger;
    protected readonly ISymbolWrapperConverter _symbolWrapperConverter;
    protected readonly IContextInfoDtoConverter<TContext, ISyntaxNodeWrapper> _contextInfoDtoConverter;

    protected ContextInfoBuilder(
        IContextCollector<TContext> collector,
        IContextFactory<TContext> factory,
        ISymbolWrapperConverter symbolWrapperConverter,
        IContextInfoDtoConverter<TContext, ISyntaxNodeWrapper> contextInfoDtoConverter,
        IAppLogger<AppLevel> logger)
    {
        _collector = collector;
        _factory = factory;
        _logger = logger;
        _symbolWrapperConverter = symbolWrapperConverter;
        _contextInfoDtoConverter = contextInfoDtoConverter;

    }

    public abstract ContextInfoElementType ElementType { get; }

    public bool CanBuild(object syntax) => syntax is TSyntaxNode;

    public abstract bool CanBuild(ISyntaxWrapper contextInfo);

    // context: ContextInfo, build
    public virtual TContext BuildContextInfo(TContext? ownerContext, object syntaxNode, ISemanticModelWrapper semanticModel, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var syntaxNodeWrapper = new TWrapper();
        syntaxNodeWrapper.SetSyntax(syntaxNode);

        var symbolInfo = _symbolWrapperConverter.Convert(semanticModel, syntaxNodeWrapper, cancellationToken);
        var dto = _contextInfoDtoConverter.Convert(ownerContext, syntaxNodeWrapper, symbolInfo, ElementType);

        return BuildContextInfo(ownerContext, dto);
    }

    // context: ContextInfo, build
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

