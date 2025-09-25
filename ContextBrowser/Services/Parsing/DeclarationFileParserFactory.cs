using ContextBrowserKit.Options;
using ContextKit.ContextData.Comment;
using ContextKit.Model;
using LoggerKit;
using RoslynKit.Phases;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace ContextBrowser.Services.Parsing;

public interface IDeclarationParserFactory
{
    ISemanticDeclarationParser<ContextInfo> Create(SemanticOptions options);
}

public class DeclarationParserFactory : IDeclarationParserFactory
{
    private readonly IContextCollector<ContextInfo> _contextInfoCollector;
    private readonly IContextFactory<ContextInfo> _contextInfoFactory;
    private readonly IContextInfoCommentProcessor<ContextInfo> _commentProcessor;
    private readonly IAppLogger<AppLevel> _appLogger;
    private readonly ISemanticTreeModelBuilder<ISyntaxTreeWrapper, ISemanticModelWrapper> _semanticTreeModelBuilder;

    public DeclarationParserFactory(
            IContextCollector<ContextInfo> contextInfoCollector,
            IContextFactory<ContextInfo> contextInfoFactory,
            IContextInfoCommentProcessor<ContextInfo> commentProcessor,
            IAppLogger<AppLevel> appLogger,
            ISemanticTreeModelBuilder<ISyntaxTreeWrapper, ISemanticModelWrapper> semanticTreeModelBuilder)
    {
        _contextInfoCollector = contextInfoCollector;
        _contextInfoFactory = contextInfoFactory;
        _commentProcessor = commentProcessor;
        _appLogger = appLogger;
        _semanticTreeModelBuilder = semanticTreeModelBuilder;
    }

    public ISemanticDeclarationParser<ContextInfo> Create(SemanticOptions options)
    {
        // 1. router (для декларативного парсера)
        var dependenciesFactory = new CSharpPhaseParserDependenciesFactory<ContextInfo>(_contextInfoCollector, _contextInfoFactory, _commentProcessor, _appLogger);
        var router = dependenciesFactory.CreateRouter(options);

        // 2. декларативный парсер
        return new SemanticDeclarationParser<ContextInfo>(
            modelBuilder: _semanticTreeModelBuilder,
            logger: _appLogger,
            router,
            _contextInfoCollector);

    }
}