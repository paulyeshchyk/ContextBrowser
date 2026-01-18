using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Collector;
using LoggerKit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SemanticKit.Model;
using SemanticKit.Parsers.Strategy.Invocation;

namespace RoslynKit.Assembly.Strategy.Invocation;

// context: roslyn, build
public class RoslynInvocationParserFactory<TSyntaxTreeWrapper> : IReferenceParserFactory<TSyntaxTreeWrapper>
    where TSyntaxTreeWrapper : ISyntaxTreeWrapper
{
    private readonly ISemanticModelStorage<TSyntaxTreeWrapper, ISemanticModelWrapper> _semanticModelStorage;
    private readonly ISyntaxTreeWrapperBuilder<TSyntaxTreeWrapper> _syntaxTreeWrapperBuilder;
    private readonly IAppLogger<AppLevel> _appLogger;
    private readonly IContextFactory<ContextInfo> _contextInfoFactory;
    private readonly IContextCollector<ContextInfo> _contextInfoCollector;
    private readonly IInvocationLinker<ContextInfo, InvocationExpressionSyntax> _invocationLinker;
    private readonly IAppOptionsStore _optionsStore;

    public RoslynInvocationParserFactory(
        ISemanticModelStorage<TSyntaxTreeWrapper, ISemanticModelWrapper> semanticModelStorage,
        ISyntaxTreeWrapperBuilder<TSyntaxTreeWrapper> syntaxTreeWrapperBuilder,
        IAppLogger<AppLevel> appLogger,
        IContextFactory<ContextInfo> contextInfoFactory,
        IContextCollector<ContextInfo> contextInfoCollector,
        IInvocationLinker<ContextInfo, InvocationExpressionSyntax> invocationLinker,
        IAppOptionsStore optionsStore)
    {
        _semanticModelStorage = semanticModelStorage;
        _syntaxTreeWrapperBuilder = syntaxTreeWrapperBuilder;
        _appLogger = appLogger;
        _contextInfoFactory = contextInfoFactory;
        _contextInfoCollector = contextInfoCollector;
        _invocationLinker = invocationLinker;
        _optionsStore = optionsStore;
    }

    // context: roslyn, build
    public IInvocationFileParser<ContextInfo> Create()
    {
        var referenceCollector = new ContextInfoReferenceCollector<ContextInfo>(_contextInfoCollector.GetAll());

        var invocationReferenceBuilder = new RoslynInvocationBuilder<ContextInfo>(
            _appLogger,
            _contextInfoFactory,
            referenceCollector,
            _invocationLinker);

        var invocationParser = new InvocationParser<ContextInfo, TSyntaxTreeWrapper>(
            _syntaxTreeWrapperBuilder,
            referenceCollector,
            _appLogger,
            _semanticModelStorage,
            invocationReferenceBuilder,
            _optionsStore
        );

        var referenceParser = new InvocationFileParser<ContextInfo, TSyntaxTreeWrapper>(
            collector: referenceCollector,
            semanticTreeModelStorage: _semanticModelStorage,
            syntaxTreeWrapperBuilder: _syntaxTreeWrapperBuilder,
            invocationReferenceBuilder: invocationReferenceBuilder,
            logger: _appLogger,
            optionsStore: _optionsStore,
            invocationParser);

        return referenceParser;
    }
}

