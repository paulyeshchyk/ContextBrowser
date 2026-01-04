using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Collector;
using LoggerKit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Assembly;
using RoslynKit.ContextInfoBuilder;
using RoslynKit.Model.Meta;
using RoslynKit.Phases.ContextInfoBuilder;
using SemanticKit.Model;

namespace RoslynKit.Assembly;

// context: roslyn, build
public class RoslynReferenceParserFactory<TSyntaxTreeWrapper> : IReferenceParserFactory<TSyntaxTreeWrapper>
    where TSyntaxTreeWrapper : ISyntaxTreeWrapper
{
    private readonly ISemanticModelStorage<TSyntaxTreeWrapper, ISemanticModelWrapper> _semanticModelStorage;
    private readonly ISyntaxTreeWrapperBuilder<TSyntaxTreeWrapper> _syntaxTreeWrapperBuilder;
    private readonly IAppLogger<AppLevel> _appLogger;
    private readonly IContextFactory<ContextInfo> _contextInfoFactory;
    private readonly IContextCollector<ContextInfo> _contextInfoCollector;
    private readonly IInvocationLinker<ContextInfo, InvocationExpressionSyntax> _invocationLinker;

    public RoslynReferenceParserFactory(
        ISemanticModelStorage<TSyntaxTreeWrapper, ISemanticModelWrapper> semanticModelStorage,
        ISyntaxTreeWrapperBuilder<TSyntaxTreeWrapper> syntaxTreeWrapperBuilder,
        IAppLogger<AppLevel> appLogger,
        IContextFactory<ContextInfo> contextInfoFactory,
        IContextCollector<ContextInfo> contextInfoCollector,
        IInvocationLinker<ContextInfo, InvocationExpressionSyntax> invocationLinker)
    {
        _semanticModelStorage = semanticModelStorage;
        _syntaxTreeWrapperBuilder = syntaxTreeWrapperBuilder;
        _appLogger = appLogger;
        _contextInfoFactory = contextInfoFactory;
        _contextInfoCollector = contextInfoCollector;
        _invocationLinker = invocationLinker;
    }

    // context: roslyn, build
    public IInvocationParser<ContextInfo> Create()
    {
        var referenceCollector = new ContextInfoReferenceCollector<ContextInfo>(_contextInfoCollector.GetAll());

        var invocationReferenceBuilder = new SemanticInvocationReferenceBuilder<ContextInfo>(
            _appLogger,
            _contextInfoFactory,
            referenceCollector,
            _invocationLinker);

        var referenceParser = new RoslynInvocationParser<ContextInfo, TSyntaxTreeWrapper>(
            collector: referenceCollector,
            semanticTreeModelStorage: _semanticModelStorage,
            syntaxTreeWrapperBuilder: _syntaxTreeWrapperBuilder,
            invocationReferenceBuilder: invocationReferenceBuilder,
            logger: _appLogger);

        return referenceParser;
    }
}

