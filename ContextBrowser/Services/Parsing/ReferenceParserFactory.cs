using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Collector;
using LoggerKit;
using RoslynKit.Phases.ContextInfoBuilder;
using RoslynKit.Phases.Invocations.Parser;
using RoslynKit.Phases.Syntax.Parsers;
using SemanticKit.Model;

namespace ContextBrowser.Services.Parsing;

public interface IReferenceParserFactory
{
    RoslynInvocationParser<ContextInfo> Create();
}

public class ReferenceParserFactory : IReferenceParserFactory
{
    private readonly ISemanticModelStorage<ISyntaxTreeWrapper, ISemanticModelWrapper> _semanticModelStorage;
    private readonly ISyntaxTreeWrapperBuilder _syntaxTreeWrapperBuilder;
    private readonly IAppLogger<AppLevel> _appLogger;
    private readonly IContextFactory<ContextInfo> _contextInfoFactory;
    private readonly IContextCollector<ContextInfo> _contextInfoCollector;
    private readonly IInvocationSyntaxResolver _invocationSyntaxExtractor;

    public ReferenceParserFactory(
        ISemanticModelStorage<ISyntaxTreeWrapper, ISemanticModelWrapper> semanticModelStorage,
        ISyntaxTreeWrapperBuilder syntaxTreeWrapperBuilder,
        IAppLogger<AppLevel> appLogger,
        IContextFactory<ContextInfo> contextInfoFactory,
        IContextCollector<ContextInfo> contextInfoCollector,
        IInvocationSyntaxResolver invocationSyntaxExtractor)
    {
        _semanticModelStorage = semanticModelStorage;
        _syntaxTreeWrapperBuilder = syntaxTreeWrapperBuilder;
        _appLogger = appLogger;
        _contextInfoFactory = contextInfoFactory;
        _contextInfoCollector = contextInfoCollector;
        _invocationSyntaxExtractor = invocationSyntaxExtractor;
    }

    public RoslynInvocationParser<ContextInfo> Create()
    {
        var referenceCollector = new ContextInfoReferenceCollector<ContextInfo>(_contextInfoCollector.GetAll());

        var invocationReferenceBuilder = new RoslynInvocationReferenceBuilder<ContextInfo>(
            _appLogger,
            _contextInfoFactory,
            _invocationSyntaxExtractor,
            referenceCollector);

        var referenceParser = new RoslynInvocationParser<ContextInfo>(
            collector: referenceCollector,
            semanticTreeModelStorage: _semanticModelStorage,
            syntaxTreeWrapperBuilder: _syntaxTreeWrapperBuilder,
            invocationReferenceBuilder: invocationReferenceBuilder,
            logger: _appLogger);

        return referenceParser;
    }
}

