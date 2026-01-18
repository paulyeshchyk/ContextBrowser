using System.Collections.Generic;
using ContextBrowser.Infrastructure;
using ContextBrowser.Services;
using ContextBrowser.Services.ContextInfoProvider;
using ContextBrowser.Services.Parsing;
using ContextBrowserKit.Options;
using ContextKit.ContextData;
using ContextKit.ContextData.Comment;
using ContextKit.ContextData.Naming;
using ContextKit.Model;
using ContextKit.Model.CacheManager;
using ContextKit.Model.Collector;
using ContextKit.Model.Factory;
using ContextKit.Model.Service;
using ContextKit.Model.WordTensorBuildStrategy;
using ExporterKit.Csv;
using ExporterKit.Html.Containers;
using ExporterKit.Html.Pages;
using ExporterKit.Html.Pages.CoCompiler;
using ExporterKit.Html.Pages.CoCompiler.DomainPerAction;
using ExporterKit.Html.Pages.CoCompiler.DomainPerAction.Coverage;
using ExporterKit.Infrastucture;
using ExporterKit.Uml;
using ExporterKit.Uml.DiagramCompileOptions;
using ExporterKit.Uml.DiagramCompileOptions.Strategies;
using ExporterKit.Uml.DiagramCompiler;
using ExporterKit.Uml.DiagramCompiler.ActionPerDomainPackage;
using ExporterKit.Uml.DiagramCompiler.CoCompiler;
using ExporterKit.Uml.DiagramCompiler.CoCompiler.Class;
using ExporterKit.Uml.DiagramCompiler.CoCompiler.Mindmap;
using ExporterKit.Uml.DiagramCompiler.CoCompiler.Package;
using HtmlKit.Document;
using HtmlKit.Helpers;
using HtmlKit.Matrix;
using LoggerKit;
using LoggerKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using RoslynKit.Assembly;
using RoslynKit.Assembly.Strategy;
using RoslynKit.Assembly.Strategy.Declaration;
using RoslynKit.Assembly.Strategy.Invocation;
using RoslynKit.Converters;
using RoslynKit.Lookup;
using RoslynKit.Model.Meta;
using RoslynKit.Model.SyntaxWrapper;
using RoslynKit.Phases;
using RoslynKit.Phases.ContextInfoBuilder;
using RoslynKit.Phases.Syntax;
using RoslynKit.Signature;
using RoslynKit.Signature.SignatureParser;
using SemanticKit.Model;
using SemanticKit.Model.Signature;
using SemanticKit.Parsers;
using SemanticKit.Parsers.File;
using SemanticKit.Parsers.Strategy;
using SemanticKit.Parsers.Strategy.Declaration;
using SemanticKit.Parsers.Strategy.Invocation;
using SemanticKit.Parsers.Syntax;
using TensorKit.Factories;
using TensorKit.Model;
using UmlKit.Builders;
using UmlKit.Builders.Strategies;
using UmlKit.Builders.TransitionDirection;
using UmlKit.Builders.TransitionFactory;
using UmlKit.Builders.Url;
using UmlKit.Compiler;
using UmlKit.Compiler.Orchestrant;
using UmlKit.DiagramGenerator.Renderer;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;
using UmlKit.Renderer;

namespace ContextBrowser;

public class HostConfigurator
{
    public static void ConfigureServices(IServiceCollection services)
    {
        // --- Общие службы и настройки приложения (Options) ---
        services.AddSingleton<IAppOptionsStore, AppSettingsStore>();

        services.AddSingleton<IAppLogger<AppLevel>, IndentedAppLogger<AppLevel>>(_ =>
        {
            return AppLoggerFactory.DefaultIndentedLogger<AppLevel>();
        });

        services.AddSingleton<INamingProcessor, NamingProcessor>();

        // --- Службы, связанные с ContextInfo и анализом кода ---
        services.AddTransient<IContextInfoManager<ContextInfo>, ContextInfoManager<ContextInfo>>();
        services.AddTransient<IContextInfoRelationBuilder<ContextInfo>, ContextInfoRelationBuilder>();
        services.AddSingleton<IContextCollector<ContextInfo>, ContextInfoCollector<ContextInfo>>();
        services.AddSingleton<ISemanticModelStorage<RoslynSyntaxTreeWrapper, ISemanticModelWrapper>, SemanticModelStorage<RoslynSyntaxTreeWrapper>>();

        services.AddSingleton<IContextFactory<ContextInfo>, ContextInfoFactory>();
        services.AddTransient<ISyntaxTreeWrapperBuilder<RoslynSyntaxTreeWrapper>, RoslynSyntaxTreeWrapperBuilder<RoslynSyntaxTreeWrapper>>();

        services.AddTransient<IContextInfoRelationOwnerInjector, ContextInfoRelationClassOwnerInjector>();
        services.AddTransient<IContextInfoRelationOwnerInjector, ContextInfoRelationMethodOwnerInjector>();

        services.AddTransient<IContextInfoRelationInjector, ContextInfoRelationReferenceInjector>();
        services.AddTransient<IContextInfoRelationInjector, ContextInfoRelationInvokedByInjector>();
        services.AddTransient<IContextInfoRelationInjector, ContextInfoRelationPropertyInjector>();
        services.AddTransient<IContextInfoRelationInjector, ContextInfoRelationOwnsInjector>();

        services.AddSingleton<IContextInfoIndexerProvider, ContextInfoIndexerProvider>();
        services.AddTransient<ICsvGenerator<DomainPerActionTensor>, CsvGenerator<DomainPerActionTensor>>();
        services.AddTransient<IFileCacheStrategy, ContextFileCacheStrategy>();
        services.AddSingleton<IContextInfoCacheService, ContextInfoCacheService>();
        services.AddTransient<IContextInfoRelationManager, ContextInfoRelationManager>();
        services.AddSingleton<IKeyIndexBuilder<ContextInfo>, HtmlMatrixIndexerByNameWithClassOwnerName<ContextInfo, DomainPerActionTensor>>();
        services.AddSingleton<IContextInfoIndexerFactory, ContextInfoFlatMapperFactory>();
        services.AddSingleton<IDictionary<MapperKeyBase, IKeyIndexBuilder<ContextInfo>>>(provider => new Dictionary<MapperKeyBase, IKeyIndexBuilder<ContextInfo>> { { GlobalMapperKeys.NameClassName, provider.GetRequiredService<IKeyIndexBuilder<ContextInfo>>() } });
        services.AddSingleton<IDictionary<MapperKeyBase, IContextInfo2DMap<ContextInfo, DomainPerActionTensor>>>(provider => new Dictionary<MapperKeyBase, IContextInfo2DMap<ContextInfo, DomainPerActionTensor>> { { GlobalMapperKeys.DomainPerAction, provider.GetRequiredService<IContextInfo2DMap<ContextInfo, DomainPerActionTensor>>() } });

        services.AddTransient<ITensorBuilder, TensorBuilder>();

        services.AddTransient<ITensorFactory<DomainPerActionTensor>>(_ => new TensorFactory<DomainPerActionTensor>(dimensions => new DomainPerActionTensor(dimensions)));
        services.AddTransient<ITensorFactory<MethodListTensor<DomainPerActionTensor>>>(_ => new TensorFactory<MethodListTensor<DomainPerActionTensor>>(dimensions => new MethodListTensor<DomainPerActionTensor>(dimensions)));

        services.AddSingleton<IContextInfoFiller<DomainPerActionTensor>, ContextInfoFiller<DomainPerActionTensor>>();
        services.AddSingleton<IContextInfoFiller<DomainPerActionTensor>, ContextInfoFillerEmptyData<DomainPerActionTensor>>();
        services.AddSingleton<IContextInfoMapperProvider<DomainPerActionTensor>, ContextInfoMappingProvider<DomainPerActionTensor>>();
        services.AddSingleton<IContextInfo2DMap<ContextInfo, DomainPerActionTensor>, ContextInfo2DMap<DomainPerActionTensor, ContextInfo>>();
        services.AddTransient<IContextInfoMapperFactory<DomainPerActionTensor>, ContextInfoMapperFactory<DomainPerActionTensor>>();
        services.AddTransient<IContextInfoDtoConverter<ContextInfo, ISyntaxNodeWrapper>, ContextInfoDtoConverter<ContextInfo>>();

        // --- Code parsing
        services.AddTransient<ICodeParseService, CodeParseService>();
        services.AddSingleton<SignatureChainFactory>();

        services.AddSingleton<ISignatureParser<CSharpIdentifier>, CSharpSignatureParser>();
        services.AddSingleton<ISignatureParserChain, CSharpSignatureParserChain>();
        services.AddSingleton<ISignatureRegexMatcher, CSharpSignatureRegexMatcher>();

        services.AddTransient<ICSharpInvocationSyntaxWrapperConverter, CSharpInvocationSyntaxWrapperConverter>();

        services.AddTransient<ICSharpSyntaxWrapperTypeBuilderDefault, CSharpSyntaxWrapperTypeBuilderDefault>();
        services.AddTransient<ICSharpSyntaxWrapperTypeBuilderSigned, CSharpSyntaxWrapperTypeBuilderSigned>();

        // --- language selector
        // использует RoslynSemanticSyntaxRouterBuilder
        // будет использовать AngularSemanticSyntaxRouterBuilder
        services.AddTransient<ISyntaxRouterBuilderRegistry<ContextInfo>, SemanticSyntaxRouterBuilderRegistry<ContextInfo>>();

        services.AddTransient<IAssemblyFetcher<MetadataReference>, RoslynAssemblyFetcher>();

        // --- Roslyn ---
        services.AddTransient<IParsingOrchestrator, ParsingOrchestrator<RoslynSyntaxTreeWrapper>>();
        services.AddTransient<IDeclarationParser<ContextInfo, RoslynSyntaxTreeWrapper>, RoslynDeclarationParser<ContextInfo>>();

        services.AddTransient<ISemanticCompilationMapBuilder<RoslynSyntaxTreeWrapper, ISemanticModelWrapper>, SemanticCompilationMapBuilder<RoslynSyntaxTreeWrapper>>();
        services.AddTransient<ISemanticCompilationViewBuilder<RoslynSyntaxTreeWrapper, ISemanticModelWrapper>, SemanticCompilationViewBuilder<RoslynSyntaxTreeWrapper>>();

        services.AddTransient<IDeclarationFileParser<ContextInfo>, DeclarationFileParser<ContextInfo, RoslynSyntaxTreeWrapper>>();
        services.AddTransient<ICompilationBuilder<RoslynSyntaxTreeWrapper>, RoslynCompilationBuilder>();
        services.AddTransient<ISyntaxCompiler<MetadataReference, RoslynSyntaxTreeWrapper, CSharpCompilation>, RoslynSyntaxCompiler>();
        services.AddTransient<ISemanticMapExtractor<RoslynSyntaxTreeWrapper>, RoslynCompilationMapBuilder>();
        services.AddTransient<ISyntaxTreeParser<RoslynSyntaxTreeWrapper>, RoslynSyntaxTreeParser>();
        services.AddTransient<ICompilationMapMapper<RoslynSyntaxTreeWrapper>, RoslynCompilationMapMapper>();
        services.AddTransient<ICompilationDiagnosticsInspector<CSharpCompilation, Diagnostic>, RoslynDiagnosticsInspector>();
        services.AddTransient<ICodeInjector, RoslynCodeInjector>();

        // invocation resolver
        services.AddTransient<ISymbolLookupHandlerChainFactory<ContextInfo, ISemanticModelWrapper>, RoslynSymbolLookupHandlerChainFactory>();
        services.AddTransient<ISymbolLookupHandler<ContextInfo, ISemanticModelWrapper>, RoslynSymbolLookupHandlerFullname<ContextInfo, ISemanticModelWrapper>>();
        services.AddTransient<ISymbolLookupHandler<ContextInfo, ISemanticModelWrapper>, RoslynSymbolLookupHandlerMethod<ContextInfo, ISemanticModelWrapper>>();
        services.AddTransient<ISymbolLookupHandler<ContextInfo, ISemanticModelWrapper>, RoslynSymbolLookupHandlerInvocation<ContextInfo, ISemanticModelWrapper>>();

        //

        services.AddTransient<IRoslynSymbolLoader<MemberDeclarationSyntax, ISymbol>, RoslynSymbolLoader<MemberDeclarationSyntax, ISymbol>>();
        services.AddTransient<IInvocationResolver<RoslynSyntaxTreeWrapper>, RoslynInvocationResolver>();
        services.AddTransient<IInvocationSyntaxResolver, RoslynInvocationSyntaxResolver>();
        services.AddTransient<IReferenceParserFactory<RoslynSyntaxTreeWrapper>, RoslynInvocationParserFactory<RoslynSyntaxTreeWrapper>>();

        services.AddTransient<ISyntaxRouterBuilder<ContextInfo>, RoslynSemanticSyntaxRouterBuilder<ContextInfo>>();

        services.AddTransient<IContextInfoBuilder<ContextInfo>, CSharpContextInfoBuilderProperty<ContextInfo>>();
        services.AddTransient<IContextInfoBuilder<ContextInfo>, CSharpContextInfoBuilderDelegate<ContextInfo>>();
        services.AddTransient<IContextInfoBuilder<ContextInfo>, CSharpContextInfoBuilderEnum<ContextInfo>>();
        services.AddTransient<IContextInfoBuilder<ContextInfo>, CSharpContextInfoBuilderInterface<ContextInfo>>();
        services.AddTransient<IContextInfoBuilder<ContextInfo>, CSharpContextInfoBuilderMethod<ContextInfo>>();
        services.AddTransient<IContextInfoBuilder<ContextInfo>, CSharpContextInfoBuilderMethodArtifitial<ContextInfo>>();
        services.AddTransient<IContextInfoBuilder<ContextInfo>, CSharpContextInfoBulderType<ContextInfo>>();
        services.AddTransient<IContextInfoBuilder<ContextInfo>, CSharpContextInfoBuilderRecord<ContextInfo>>();
        services.AddTransient<ContextInfoBuilderDispatcher<ContextInfo>>();

        services.AddTransient<IInvocationLinksBuilder<ContextInfo>, RoslynInvocationLinksBuilder>();
        services.AddTransient<IInvocationLinker<ContextInfo, InvocationExpressionSyntax>, RoslynInvocationLinker<ContextInfo>>();
        services.AddTransient<ISymbolWrapperConverter, RoslynSymbolWrapperConverter>();

        // ---

        services.AddSingleton<IContextInfoCommentProcessor<ContextInfo>, ContextInfoCommentProcessor<ContextInfo>>();
        services.AddTransient(typeof(IContextInfoCommentProcessor<>), typeof(ContextInfoCommentProcessor<>));
        services.AddTransient(typeof(ICommentParsingStrategyFactory<>), typeof(CommentParsingStrategyFactory<>));

        services.AddTransient<IContextElementExtractor<ContextInfo>, ContextElementExtractor<ContextInfo>>();
        services.AddTransient<IWordTensorBuildStrategy<DomainPerActionTensor>, WordTensorBuildStrategyVerbNoun<DomainPerActionTensor>>();
        services.AddTransient<IWordTensorBuildStrategy<DomainPerActionTensor>, WordTensorBuildStrategyVerbOnly<DomainPerActionTensor>>();
        services.AddTransient<IWordTensorBuildStrategy<DomainPerActionTensor>, WordTensorBuildStrategyNounOnly<DomainPerActionTensor>>();
        services.AddTransient<IWordTensorBuildStrategy<DomainPerActionTensor>, WordTensorBuildStrategyUnclassified<DomainPerActionTensor>>();

        // --- Службы, связанные с генерацией HTML ---

        services.AddTransient<IHtmlCompilerOrchestrator, HtmlCompilerOrchestrator>();
        services.AddTransient<IHtmlPageCompiler, HtmlPageCompilerIndexDomainPerAction<DomainPerActionTensor>>();
        services.AddTransient<IHtmlPageCompiler, HtmlPageCompilerNamespaceOnly<DomainPerActionTensor>>();
        services.AddTransient<IHtmlPageCompiler, HtmlPageCompilerClassOnly<DomainPerActionTensor>>();
        services.AddTransient<IHtmlPageCompiler, HtmlPageCompilerActionPerDomain<DomainPerActionTensor>>();
        services.AddTransient<IHtmlPageCompiler, HtmlPageCompilerActionOnly<DomainPerActionTensor>>();
        services.AddTransient<IHtmlPageCompiler, HtmlPageCompilerDomainOnly<DomainPerActionTensor>>();
        services.AddTransient<IHtmlPageCompiler, HtmlPageCompilerActionPerDomainSummary<DomainPerActionTensor>>();

        services.AddTransient<IHtmlMatrixGenerator, HtmlMatrixGenerator<DomainPerActionTensor>>();

        services.AddTransient<IContextInfoDatasetBuilder<DomainPerActionTensor>, ContextInfoDatasetBuilder<DomainPerActionTensor>>();
        services.AddTransient<IHtmlTensorWriter<DomainPerActionTensor>, HtmlTensorWriter<DomainPerActionTensor>>();
        services.AddTransient<IHtmlDataCellBuilder<DomainPerActionTensor>, HtmlDataCellBuilderCoverage<DomainPerActionTensor>>();
        services.AddSingleton<IContextInfoDatasetProvider<DomainPerActionTensor>, ContextInfoDatasetProvider<DomainPerActionTensor>>();

        services.AddTransient<IContextInfoDatasetBuilder<MethodListTensor<DomainPerActionTensor>>, ContextInfoDatasetBuilder<MethodListTensor<DomainPerActionTensor>>>();
        services.AddTransient<IHtmlTensorWriter<MethodListTensor<DomainPerActionTensor>>, HtmlTensorWriter<MethodListTensor<DomainPerActionTensor>>>();
        services.AddTransient<IHtmlDataCellBuilder<MethodListTensor<DomainPerActionTensor>>, HtmlDataCellBuilderMethodList<DomainPerActionTensor>>();
        services.AddSingleton<IContextInfoDatasetProvider<MethodListTensor<DomainPerActionTensor>>, ContextInfoDatasetProviderMethodList<DomainPerActionTensor>>();

        services.AddTransient<IHtmlPageIndexProducer<DomainPerActionTensor>, HtmlPageProducerIndex<DomainPerActionTensor>>();

        services.AddTransient<IHtmlFixedContentManager, HtmlFixedContentManagerDomainPerAction>();

        services.AddTransient<IHtmlHrefManager<DomainPerActionTensor>, HtmlHrefManagerDomainPerAction>();
        services.AddTransient<IHtmlHrefManager<MethodListTensor<DomainPerActionTensor>>, HtmlHrefManagerMethodList<DomainPerActionTensor>>();

        services.AddTransient<IHtmlCellDataProducer<string, DomainPerActionTensor>, HtmlCellDataProducerDomainPerActionMethodsCount<DomainPerActionTensor>>();
        services.AddTransient<IHtmlCellDataProducer<List<ContextInfo>, DomainPerActionTensor>, HtmlCellDataProducerListOfItems<DomainPerActionTensor>>();
        services.AddTransient<IHtmlContentInjector<DomainPerActionTensor>, HtmlContentInjector<DomainPerActionTensor>>();

        services.AddTransient<IHtmlCellDataProducer<string, MethodListTensor<DomainPerActionTensor>>, HtmlCellDataProducerMethodList<MethodListTensor<DomainPerActionTensor>>>();
        services.AddTransient<IHtmlCellDataProducer<List<ContextInfo>, MethodListTensor<DomainPerActionTensor>>, HtmlCellDataProducerListOfItems<MethodListTensor<DomainPerActionTensor>>>();
        services.AddTransient<IHtmlContentInjector<MethodListTensor<DomainPerActionTensor>>, HtmlContentInjector<MethodListTensor<DomainPerActionTensor>>>();

        services.AddScoped<IHtmlCellStyleBuilder<DomainPerActionTensor>, HtmlCellStyleBuilder<DomainPerActionTensor>>();
        services.AddScoped<IHtmlCellColorCalculator<DomainPerActionTensor>, HtmlCellColorCalculatorCoverage<DomainPerActionTensor>>();

        services.AddTransient<IHtmlMatrixSummaryBuilder<DomainPerActionTensor>, HtmlMatrixSummaryBuilder<DomainPerActionTensor>>();
        services.AddScoped<ICoverageValueExtractor, CoverageValueExtractor>();

        // --- Службы, связанные с генерацией UML ---

        services.AddTransient<IDiagramCompileOptionsStrategy, ActionStateCompileOptionsStrategy>();
        services.AddTransient<IDiagramCompileOptionsStrategy, DomainStateCompileOptionsStrategy>();
        services.AddTransient<IDiagramCompileOptionsStrategy, ActionSequenceCompileOptionsStrategy>();
        services.AddTransient<IDiagramCompileOptionsStrategy, DomainSequenceCompileOptionsStrategy>();

        // регистрация резолвера/фабрики

        services.AddTransient<IUmlTransitionFactory<UmlState>, UmlTransitionStateFactory<UmlState>>();
        services.AddTransient<IUmlTransitionFactory<UmlParticipant>, UmlTransitionParticipantFactory<UmlParticipant>>();

        services.AddTransient<IDiagramCompileOptionsFactory, DiagramCompileOptionsFactory>();

        services.AddTransient<IUmlUrlBuilder, UmlUrlBuilder>();

        // renderer
        services.AddTransient<UmlTransitionRendererFlat<UmlParticipant>, UmlTransitionRendererFlatParticipant>();
        services.AddTransient<UmlTransitionRendererFlat<UmlState>, UmlTransitionRendererFlatState>();
        services.AddTransient<UmlClassRendererActionPerDomainPackageMethod>();
        services.AddTransient<UmlClassRendererActionPerDomainClass>();
        services.AddTransient<UmlClassRendererMethods>();
        services.AddTransient<UmlClassRendererClassOnly>();
        services.AddTransient<UmlClassRendererLinks>();
        services.AddTransient<UmlMindmapRendererAction>();
        services.AddTransient<UmlMindmapRendererClassOnly>();
        services.AddTransient<UmlMindmapRendererDomain>();
        services.AddTransient<UmlClassRendererPackages>();
        services.AddTransient(typeof(UmlClassRendererNamespace<>));

        services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerClassActionPerDomain<DomainPerActionTensor>>();
        services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerNamespaceOnly<DomainPerActionTensor>>();
        services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerClassMethodsList>();
        services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerClassRelation>();
        services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerClassOnly>();

        services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerActionPerDomainPackageDomainGroup>();
        services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerActionPerDomainPackageNoDomainGroup>();
        services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerActionPerDomainPackageActionGroup>();
        services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerActionPerDomainPackageNoActionGroup>();

        services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerPackages>();
        services.AddTransient<IUmlDiagramCompiler, CompositeUmlDiagramCompilerSequenceAction>();
        services.AddTransient<IUmlDiagramCompiler, CompositeUmlDiagramCompilerSequenceDomain>();
        services.AddTransient<IUmlDiagramCompiler, CompositeUmlDiagramCompilerStateAction>();
        services.AddTransient<IUmlDiagramCompiler, CompositeUmlDiagramCompilerStateDomain>();
        services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerMindmapDomain>();
        services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerMindmapAction>();
        services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerMindmapClassOnly>();
        services.AddTransient<IUmlDiagramCompilerOrchestrator, UmlDiagramCompilerOrchestrator>();

        services.AddTransient<ITransitionBuilder, OutgoingTransitionBuilder>();
        services.AddTransient<ITransitionBuilder, IncomingTransitionBuilder>();
        services.AddTransient<ITransitionBuilder, BiDirectionalTransitionBuilder>();

        services.AddSingleton<IDiagramBuilderRegistration, TransitionDiagramBuilderRegistration>();

        services.AddSingleton<IContextDiagramBuildersFactory, ContextDiagramBuildersFactory>();
    }
}
