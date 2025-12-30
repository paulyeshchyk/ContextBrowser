using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommandlineKit;
using ContextBrowser.Infrastructure;
using ContextBrowser.Services;
using ContextBrowser.Services.ContextInfoProvider;
using ContextBrowser.Services.Parsing;
using ContextBrowserKit.Options;
using ContextKit.ContextData.Comment;
using ContextKit.ContextData.Naming;
using ContextKit.Model;
using ContextKit.Model.CacheManager;
using ContextKit.Model.Collector;
using ContextKit.Model.Factory;
using ContextKit.Model.WordTensorBuildStrategy;
using ExporterKit.Csv;
using ExporterKit.Html.Containers;
using ExporterKit.Html.Pages;
using ExporterKit.Html.Pages.CoCompiler;
using ExporterKit.Html.Pages.CoCompiler.DomainPerAction;
using ExporterKit.Html.Pages.CoCompiler.DomainPerAction.Coverage;
using ExporterKit.Infrastucture;
using ExporterKit.Uml;
using HtmlKit.Document;
using HtmlKit.Helpers;
using HtmlKit.Matrix;
using LoggerKit;
using LoggerKit.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RoslynKit.Assembly;
using RoslynKit.Phases;
using RoslynKit.Phases.Invocations;
using RoslynKit.Tree;
using RoslynKit.Wrappers.Extractor;
using SemanticKit.Model;
using TensorKit.Factories;
using TensorKit.Model;
using UmlKit.Compiler;
using UmlKit.Compiler.Orchestrant;

namespace ContextBrowser;

// context: app, model
public static class Program
{
    // context: app, execute
    public static async Task Main(string[] args)
    {
        var hab = Host.CreateApplicationBuilder(args);

        // --- Общие службы и настройки приложения (Options) ---

        hab.Services.AddHostedService<CustomEnvironmentHostedService>();
        hab.Services.AddSingleton<IServerStartSignal, ServerStartSignal>();
        hab.Services.AddSingleton<IAppOptionsStore, AppSettingsStore>();
        hab.Services.AddSingleton<ICommandlineArgumentsParserService, CommandlineArgumentsParserService>();
        hab.Services.AddSingleton<IMainService, MainService>();
        hab.Services.AddSingleton<IAppLogger<AppLevel>, IndentedAppLogger<AppLevel>>(_ =>
        {
            var defaultLogLevels = new AppLoggerLevelStore<AppLevel>();
            var defaultConsoleWriter = new ConsoleLogWriter();
            var defaultDependencies = new Dictionary<AppLevel, AppLevel>();
            return new IndentedAppLogger<AppLevel>(defaultLogLevels, defaultConsoleWriter, dependencies: defaultDependencies);
        });


        hab.Services.AddSingleton<INamingProcessor, NamingProcessor>();

        // --- Службы, связанные с ContextInfo и анализом кода ---

        hab.Services.AddSingleton<IContextCollector<ContextInfo>, ContextInfoCollector<ContextInfo>>();
        hab.Services.AddSingleton<ISemanticModelStorage<ISyntaxTreeWrapper, ISemanticModelWrapper>, SemanticModelStorage>();
        hab.Services.AddTransient<ISemanticTreeModelBuilder<ISyntaxTreeWrapper, ISemanticModelWrapper>, SemanticTreeModelBuilder>();
        hab.Services.AddSingleton<IContextFactory<ContextInfo>, ContextInfoFactory<ContextInfo>>();
        hab.Services.AddTransient<ISyntaxTreeWrapperBuilder, RoslynSyntaxTreeWrapperBuilder>();

        hab.Services.AddSingleton<IContextInfoIndexerProvider, ContextInfoIndexerProvider>();
        hab.Services.AddTransient<ICsvGenerator<DomainPerActionTensor>, CsvGenerator<DomainPerActionTensor>>();
        hab.Services.AddTransient<IFileCacheStrategy, ContextFileCacheStrategy>();
        hab.Services.AddSingleton<IContextInfoCacheService, ContextInfoCacheService>();
        hab.Services.AddTransient<IContextInfoRelationManager, ContextInfoRelationManager>();
        hab.Services.AddSingleton<IKeyIndexBuilder<ContextInfo>, HtmlMatrixIndexerByNameWithClassOwnerName<ContextInfo, DomainPerActionTensor>>();
        hab.Services.AddSingleton<IContextInfoIndexerFactory, ContextInfoFlatMapperFactory>();
        hab.Services.AddSingleton<IDictionary<MapperKeyBase, IKeyIndexBuilder<ContextInfo>>>(provider => new Dictionary<MapperKeyBase, IKeyIndexBuilder<ContextInfo>> { { GlobalMapperKeys.NameClassName, provider.GetRequiredService<IKeyIndexBuilder<ContextInfo>>() } });
        hab.Services.AddSingleton<IDictionary<MapperKeyBase, IContextInfo2DMap<ContextInfo, DomainPerActionTensor>>>(provider => new Dictionary<MapperKeyBase, IContextInfo2DMap<ContextInfo, DomainPerActionTensor>> { { GlobalMapperKeys.DomainPerAction, provider.GetRequiredService<IContextInfo2DMap<ContextInfo, DomainPerActionTensor>>() } });

        hab.Services.AddTransient<ITensorBuilder, TensorBuilder>();

        hab.Services.AddTransient<ITensorFactory<DomainPerActionTensor>>(_ => new TensorFactory<DomainPerActionTensor>(dimensions => new DomainPerActionTensor(dimensions)));
        hab.Services.AddTransient<ITensorFactory<MethodListTensor<DomainPerActionTensor>>>(_ => new TensorFactory<MethodListTensor<DomainPerActionTensor>>(dimensions => new MethodListTensor<DomainPerActionTensor>(dimensions)));

        hab.Services.AddSingleton<IContextInfoFiller<DomainPerActionTensor>, ContextInfoFiller<DomainPerActionTensor>>();
        hab.Services.AddSingleton<IContextInfoFiller<DomainPerActionTensor>, ContextInfoFillerEmptyData<DomainPerActionTensor>>();
        hab.Services.AddSingleton<IContextInfoMapperProvider<DomainPerActionTensor>, ContextInfoMappingProvider<DomainPerActionTensor>>();
        hab.Services.AddSingleton<IContextInfo2DMap<ContextInfo, DomainPerActionTensor>, ContextInfo2DMap<DomainPerActionTensor>>();
        hab.Services.AddTransient<IContextInfoMapperFactory<DomainPerActionTensor>, ContextInfoMapperFactory<DomainPerActionTensor>>();

        // --- Code parsing
        hab.Services.AddTransient<ICodeParseService, CodeParseService>();
        hab.Services.AddTransient<IParsingOrchestrator, ParsingOrchestrator>();
        hab.Services.AddTransient<ISemanticDeclarationParser<ContextInfo>, SemanticDeclarationParser<ContextInfo>>();

        // --- language selector
        // использует RoslynSemanticSyntaxRouterBuilder
        // будет использовать AngularSemanticSyntaxRouterBuilder
        hab.Services.AddTransient<ISemanticSyntaxRouterBuilderRegistry<ContextInfo>, SemanticSyntaxRouterBuilderRegistry<ContextInfo>>();

        // --- Roslyn ---
        hab.Services.AddTransient<ICompilationBuilder, RoslynCompilationBuilder>();
        hab.Services.AddTransient<ISemanticInvocationResolver, RoslynInvocationSemanticResolver>();
        hab.Services.AddTransient<IInvocationSyntaxResolver, RoslynInvocationSyntaxExtractor>();
        hab.Services.AddTransient<IReferenceParserFactory, RolsynReferenceParserFactory>();

        hab.Services.AddTransient<ISemanticSyntaxRouterBuilder<ContextInfo>, RoslynSemanticSyntaxRouterBuilder<ContextInfo>>();

        // ---

        hab.Services.AddSingleton<IContextInfoCommentProcessor<ContextInfo>, ContextInfoCommentProcessor<ContextInfo>>();
        hab.Services.AddTransient(typeof(IContextInfoCommentProcessor<>), typeof(ContextInfoCommentProcessor<>));
        hab.Services.AddTransient(typeof(ICommentParsingStrategyFactory<>), typeof(CommentParsingStrategyFactory<>));

        hab.Services.AddTransient<IContextElementExtractor, ContextElementExtractor>();
        hab.Services.AddTransient<IWordTensorBuildStrategy<DomainPerActionTensor>, WordTensorBuildStrategyVerbNoun<DomainPerActionTensor>>();
        hab.Services.AddTransient<IWordTensorBuildStrategy<DomainPerActionTensor>, WordTensorBuildStrategyVerbOnly<DomainPerActionTensor>>();
        hab.Services.AddTransient<IWordTensorBuildStrategy<DomainPerActionTensor>, WordTensorBuildStrategyNounOnly<DomainPerActionTensor>>();
        hab.Services.AddTransient<IWordTensorBuildStrategy<DomainPerActionTensor>, WordTensorBuildStrategyUnclassified<DomainPerActionTensor>>();

        // --- Службы, связанные с генерацией HTML ---

        hab.Services.AddTransient<IHtmlCompilerOrchestrator, HtmlCompilerOrchestrator>();
        hab.Services.AddTransient<IHtmlPageCompiler, HtmlPageCompilerIndexDomainPerAction<DomainPerActionTensor>>();
        hab.Services.AddTransient<IHtmlPageCompiler, HtmlPageCompilerNamespaceOnly<DomainPerActionTensor>>();
        hab.Services.AddTransient<IHtmlPageCompiler, HtmlPageCompilerClassOnly<DomainPerActionTensor>>();
        hab.Services.AddTransient<IHtmlPageCompiler, HtmlPageCompilerActionPerDomain<DomainPerActionTensor>>();
        hab.Services.AddTransient<IHtmlPageCompiler, HtmlPageCompilerActionOnly<DomainPerActionTensor>>();
        hab.Services.AddTransient<IHtmlPageCompiler, HtmlPageCompilerDomainOnly<DomainPerActionTensor>>();
        hab.Services.AddTransient<IHtmlPageCompiler, HtmlPageCompilerActionPerDomainSummary<DomainPerActionTensor>>();

        hab.Services.AddTransient<IHtmlMatrixGenerator, HtmlMatrixGenerator<DomainPerActionTensor>>();

        hab.Services.AddTransient<IContextInfoDatasetBuilder<DomainPerActionTensor>, ContextInfoDatasetBuilder<DomainPerActionTensor>>();
        hab.Services.AddTransient<IHtmlTensorWriter<DomainPerActionTensor>, HtmlTensorWriter<DomainPerActionTensor>>();
        hab.Services.AddTransient<IHtmlDataCellBuilder<DomainPerActionTensor>, HtmlDataCellBuilderCoverage<DomainPerActionTensor>>();
        hab.Services.AddSingleton<IContextInfoDatasetProvider<DomainPerActionTensor>, ContextInfoDatasetProvider<DomainPerActionTensor>>();

        hab.Services.AddTransient<IContextInfoDatasetBuilder<MethodListTensor<DomainPerActionTensor>>, ContextInfoDatasetBuilder<MethodListTensor<DomainPerActionTensor>>>();
        hab.Services.AddTransient<IHtmlTensorWriter<MethodListTensor<DomainPerActionTensor>>, HtmlTensorWriter<MethodListTensor<DomainPerActionTensor>>>();
        hab.Services.AddTransient<IHtmlDataCellBuilder<MethodListTensor<DomainPerActionTensor>>, HtmlDataCellBuilderMethodList<DomainPerActionTensor>>();
        hab.Services.AddSingleton<IContextInfoDatasetProvider<MethodListTensor<DomainPerActionTensor>>, ContextInfoDatasetProviderMethodList<DomainPerActionTensor>>();

        hab.Services.AddTransient<IHtmlPageIndexProducer<DomainPerActionTensor>, HtmlPageProducerIndex<DomainPerActionTensor>>();

        hab.Services.AddTransient<IHtmlFixedContentManager, HtmlFixedContentManagerDomainPerAction>();

        hab.Services.AddTransient<IHtmlHrefManager<DomainPerActionTensor>, HtmlHrefManagerDomainPerAction>();
        hab.Services.AddTransient<IHtmlHrefManager<MethodListTensor<DomainPerActionTensor>>, HtmlHrefManagerMethodList<DomainPerActionTensor>>();

        hab.Services.AddTransient<IHtmlCellDataProducer<string, DomainPerActionTensor>, HtmlCellDataProducerDomainPerActionMethodsCount<DomainPerActionTensor>>();
        hab.Services.AddTransient<IHtmlCellDataProducer<List<ContextInfo>, DomainPerActionTensor>, HtmlCellDataProducerListOfItems<DomainPerActionTensor>>();
        hab.Services.AddTransient<IHtmlContentInjector<DomainPerActionTensor>, HtmlContentInjector<DomainPerActionTensor>>();

        hab.Services.AddTransient<IHtmlCellDataProducer<string, MethodListTensor<DomainPerActionTensor>>, HtmlCellDataProducerMethodList<MethodListTensor<DomainPerActionTensor>>>();
        hab.Services.AddTransient<IHtmlCellDataProducer<List<ContextInfo>, MethodListTensor<DomainPerActionTensor>>, HtmlCellDataProducerListOfItems<MethodListTensor<DomainPerActionTensor>>>();
        hab.Services.AddTransient<IHtmlContentInjector<MethodListTensor<DomainPerActionTensor>>, HtmlContentInjector<MethodListTensor<DomainPerActionTensor>>>();

        hab.Services.AddScoped<IHtmlCellStyleBuilder<DomainPerActionTensor>, HtmlCellStyleBuilder<DomainPerActionTensor>>();
        hab.Services.AddScoped<IHtmlCellColorCalculator<DomainPerActionTensor>, HtmlCellColorCalculatorCoverage<DomainPerActionTensor>>();

        hab.Services.AddTransient<IHtmlMatrixSummaryBuilder<DomainPerActionTensor>, HtmlMatrixSummaryBuilder<DomainPerActionTensor>>();
        hab.Services.AddScoped<ICoverageValueExtractor, CoverageValueExtractor>();

        // --- Службы, связанные с генерацией UML ---

        hab.Services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerClassActionPerDomain<DomainPerActionTensor>>();
        hab.Services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerNamespaceOnly<DomainPerActionTensor>>();
        hab.Services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerClassMethodsList>();
        hab.Services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerClassRelation>();
        hab.Services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerClassOnly>();
        hab.Services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerPackageMethodPerActionDomain>();
        hab.Services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerPackages>();
        hab.Services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerSequenceAction>();
        hab.Services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerSequenceDomain>();
        hab.Services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerStateAction>();
        hab.Services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerStateDomain>();
        hab.Services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerMindmapDomain>();
        hab.Services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerMindmapAction>();
        hab.Services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerMindmapClassOnly>();
        hab.Services.AddTransient<IUmlDiagramCompilerOrchestrator, UmlDiagramCompilerOrchestrator>();

        var tokenSource = new CancellationTokenSource();

        var host = hab.Build();

        var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
        Console.CancelKeyPress += (_, e) =>
        {
            Console.WriteLine("Прервано по требованию");
            e.Cancel = true;
            lifetime.StopApplication();
        };

        var parser = host.Services.GetRequiredService<ICommandlineArgumentsParserService>();
        var options = parser.Parse<AppOptions>(args);
        if (options == null)
        {
            return;
        }

        var optionsStore = host.Services.GetRequiredService<IAppOptionsStore>();
        optionsStore.SetOptions(options);

        var logger = host.Services.GetRequiredService<IAppLogger<AppLevel>>();
        logger.Configure(options.LogConfiguration);

        var mainService = host.Services.GetRequiredService<IMainService>();

        await host.StartAsync(tokenSource.Token).ConfigureAwait(false);

        try
        {
            await mainService.RunAsync(lifetime.ApplicationStopping);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            lifetime.StopApplication();
            await host.WaitForShutdownAsync(tokenSource.Token).ConfigureAwait(false);
            tokenSource.Dispose();
        }
    }
}

public class ContextInfoMapperFactory<TTensor> : IContextInfoMapperFactory<TTensor>
    where TTensor : notnull
{
    private readonly IDictionary<MapperKeyBase, IContextInfo2DMap<ContextInfo, TTensor>> _mappers;

    public ContextInfoMapperFactory(IDictionary<MapperKeyBase, IContextInfo2DMap<ContextInfo, TTensor>> mappers)
    {
        _mappers = mappers;
    }

    public IContextInfo2DMap<ContextInfo, TTensor> GetMapper(MapperKeyBase type)
    {
        return _mappers.TryGetValue(type, out var mapper)
            ? mapper
            : throw new NotImplementedException($"Mapper for type {type} is not registered.");
    }
}

public class ContextInfoFlatMapperFactory : IContextInfoIndexerFactory
{
    private readonly IDictionary<MapperKeyBase, IKeyIndexBuilder<ContextInfo>> _mappers;

    public ContextInfoFlatMapperFactory(IDictionary<MapperKeyBase, IKeyIndexBuilder<ContextInfo>> mappers)
    {
        _mappers = mappers;
    }

    public IKeyIndexBuilder<ContextInfo> GetMapper(MapperKeyBase type)
    {
        return _mappers.TryGetValue(type, out var mapper)
            ? mapper
            : throw new NotImplementedException($"Mapper for type {type} is not registered.");
    }
}