using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading;
using System.Threading.Tasks;
using CommandlineKit;
using ContextBrowser.FileManager;
using ContextBrowser.Infrastructure;
using ContextBrowser.Samples.HtmlPages;
using ContextBrowser.Services;
using ContextBrowser.Services.ContextInfoProvider;
using ContextBrowserKit.Factories;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Collector;
using ContextKit.Model.Factory;
using ContextKit.Stategies;
using ExporterKit.Html;
using ExporterKit.Html.Pages.CoCompiler.DomainPerAction;
using ExporterKit.Html.Pages.CoCompiler.DomainPerAction.Coverage;
using ExporterKit.Html.Pages.MatrixCellSummary;
using ExporterKit.Infrastucture;
using ExporterKit.Uml;
using HtmlKit.Document;
using HtmlKit.Document.Coverage;
using HtmlKit.Extensions;
using HtmlKit.Helpers;
using HtmlKit.Model;
using HtmlKit.Page.Compiler;
using HtmlKit.Writer;
using LoggerKit;
using LoggerKit.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RoslynKit.Assembly;
using RoslynKit.Phases.Invocations;
using RoslynKit.Tree;
using RoslynKit.Wrappers.Extractor;
using SemanticKit.Model;
using UmlKit.Compiler;
using UmlKit.Compiler.Orchestrant;
using UmlKit.Exporter;
using UmlKit.Infrastructure.Options;

namespace ContextBrowser;

// context: app, model
public static class Program
{
    // context: app, execute
    public static async Task Main(string[] args)
    {
        var hab = Host.CreateApplicationBuilder(args);

        hab.Services.AddHostedService<CustomEnvironmentHostedService>();
        hab.Services.AddSingleton<IServerStartSignal, ServerStartSignal>();

        hab.Services.AddSingleton<IAppOptionsStore, AppSettingsStore>();
        hab.Services.AddSingleton<ICommandlineArgumentsParserService, CommandlineArgumentsParserService>();
        hab.Services.AddSingleton<IMainService, MainService>();
        hab.Services.AddSingleton<IContextCollector<ContextInfo>, ContextInfoCollector<ContextInfo>>();
        hab.Services.AddSingleton<ISemanticModelStorage<ISyntaxTreeWrapper, ISemanticModelWrapper>, SemanticModelStorage>();
        hab.Services.AddTransient<ISemanticTreeModelBuilder<ISyntaxTreeWrapper, ISemanticModelWrapper>, SemanticTreeModelBuilder>();
        hab.Services.AddSingleton<IContextFactory<ContextInfo>, ContextInfoFactory<ContextInfo>>();
        hab.Services.AddTransient<ISyntaxTreeWrapperBuilder, RoslynSyntaxTreeWrapperBuilder>();

        hab.Services.AddTransient<IFileCacheStrategy, ContextFileCacheStrategy>();
        hab.Services.AddSingleton<IContextInfoCacheService, ContextInfoCacheService>();

        hab.Services.AddTransient<IContextClassifierBuilder, ContextClassifierBuilder>();
        hab.Services.AddTransient<IContextInfoRelationManager, ContextInfoRelationManager>();

        hab.Services.AddSingleton<IContextInfoFiller, ContextInfoFillerMatrixData>();
        hab.Services.AddSingleton<IContextInfoFiller, ContextInfoFillerEmptydata>();

        hab.Services.AddSingleton<IContextKeyIndexer<ContextInfo>, HtmlMatrixIndexerByNameWithClassOwnerName<ContextInfo>>();
        hab.Services.AddSingleton<IContextInfoIndexerFactory, ContextInfoFlatMapperFactory>();
        hab.Services.AddSingleton<IDictionary<MapperKeyBase, IContextKeyIndexer<ContextInfo>>>(provider =>
        {
            return new Dictionary<MapperKeyBase, IContextKeyIndexer<ContextInfo>>
            {
                { GlobalMapperKeys.NameClassName, provider.GetRequiredService<IContextKeyIndexer<ContextInfo>>() }
            };
        });

        hab.Services.AddSingleton<IContextKeyMap<ContextInfo, IContextKey>, ContextInfoMapperDomainPerAction>();
        hab.Services.AddTransient<IContextInfoMapperFactory, ContextInfoMapperFactory>();

        hab.Services.AddSingleton<IDictionary<MapperKeyBase, IContextKeyMap<ContextInfo, IContextKey>>>(provider =>
        {
            return new Dictionary<MapperKeyBase, IContextKeyMap<ContextInfo, IContextKey>>
            {
                { GlobalMapperKeys.DomainPerAction, provider.GetRequiredService<IContextKeyMap<ContextInfo, IContextKey>>() }
            };
        });

        hab.Services.AddTransient<IContextInfoDatasetBuilder, ContextInfoDatasetBuilder>();

        hab.Services.AddSingleton<IContextInfoMapperProvider, ContextInfoMappingProvider>();
        hab.Services.AddSingleton<IContextInfoIndexerProvider, ContextInfoIndexerProvider>();

        hab.Services.AddSingleton<ICompilationBuilder, RoslynCompilationBuilder>();
        hab.Services.AddTransient<ICodeParseService, CodeParseService>();
        hab.Services.AddSingleton<ISemanticInvocationResolver, RoslynInvocationSemanticResolver>();
        hab.Services.AddSingleton<IInvocationSyntaxResolver, RoslynInvocationSyntaxExtractor>();

        hab.Services.AddSingleton<IContextInfoCommentProcessor<ContextInfo>, ContextInfoCommentProcessor<ContextInfo>>();
        hab.Services.AddTransient(typeof(IContextInfoCommentProcessor<>), typeof(ContextInfoCommentProcessor<>));
        hab.Services.AddTransient(typeof(ICommentParsingStrategyFactory<>), typeof(CommentParsingStrategyFactory<>));

        hab.Services.AddTransient<IReferenceParserFactory, ReferenceParserFactory>();
        hab.Services.AddTransient<IDeclarationParserFactory, DeclarationParserFactory>();
        hab.Services.AddTransient<IParsingOrchestrator, ParsingOrchestrator>();

        hab.Services.AddSingleton<IContextInfoDatasetProvider, ContextInfoDatasetProvider>();

        hab.Services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerClassActionPerDomain>();
        hab.Services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerNamespaceOnly>();
        hab.Services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerClassMethodsList>();
        hab.Services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerClassRelation>();
        hab.Services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerClassOnly>();
        hab.Services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerPackageMethodPerActionDomain>();
        hab.Services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerPackages>();
        hab.Services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerSequenceAction>();
        hab.Services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerSequenceDomain>();
        hab.Services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerStateAction>();
        hab.Services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerStateDomain>();
        hab.Services.AddTransient<IUmlDiagramCompiler, UmlDiagramCompilerMindmap>();

        hab.Services.AddTransient<IUmlDiagramCompilerOrchestrator, UmlDiagramCompilerOrchestrator>();

        hab.Services.AddTransient<IFixedHtmlContentManager, FixedHtmlContentManagerDomainPerAction>();
        hab.Services.AddTransient<IHtmlContentInjector, HtmlContentInjector>();
        hab.Services.AddTransient<IHrefManager, HrefManagerDomainPerAction>();

        hab.Services.AddTransient<IHtmlCellDataProducer<string>, HtmlCellDataProducerDomainPerAction>();
        hab.Services.AddTransient<IHtmlCellDataProducer<List<ContextInfo>>, HtmlCellDataProducer>();

        //
        // Построитель таблицы(HtmlMatrixWriter) для отображения к-ва методов в пересечении домена-действия с выводом coverage
        // и сопутствующие этому модули
        //
        hab.Services.AddTransient<IContextKeyFactory<ContextKey>>(provider => new ContextKeyFactory<ContextKey>((r, c) => new ContextKey(r, c)));
        hab.Services.AddTransient<IContextKeyBuilder, ContextKeyBuilder>();
        hab.Services.AddTransient<IHtmlDataCellBuilder<ContextKey>, HtmlDataCellBuilderCoverage>();
        hab.Services.AddTransient<IHtmlMatrixWriter, HtmlMatrixWriter<ContextKey>>();

        //

        hab.Services.AddTransient<IHtmlMatrixSummaryBuilder, HtmlMatrixSummaryBuilderDomainPerAction>();

        hab.Services.AddScoped<ICoverageValueExtractor, CoverageValueExtractor>();
        hab.Services.AddScoped<IHtmlCellColorCalculator, HtmlCellColorCalculatorCoverage>();
        hab.Services.AddScoped<IHtmlCellStyleBuilder, HtmlCellStyleBuilder>();

        // Регистрация IHtmlPageMatrix (HtmlPageProducerMatrix) как транзитного сервиса.
        hab.Services.AddTransient<IHtmlPageIndex, HtmlPageProducerIndex>();

        hab.Services.AddTransient<IHtmlPageCompiler, HtmlPageCompilerDomainPerAction>();
        hab.Services.AddTransient<IHtmlPageCompiler, HtmlPageCompilerNamespaceOnly>();
        hab.Services.AddTransient<IHtmlPageCompiler, HtmlPageCompilerClassOnly>();
        hab.Services.AddTransient<IHtmlPageCompiler, HtmlPageCompilerActionPerDomain>();
        hab.Services.AddTransient<IHtmlPageCompiler, HtmlPageCompilerActionOnly>();
        hab.Services.AddTransient<IHtmlPageCompiler, HtmlPageCompilerDomainOnly>();
        hab.Services.AddTransient<IHtmlPageCompiler, HtmlPageCompilerActionPerDomainSummary>();

        hab.Services.AddTransient<IHtmlCompilerOrchestrator, HtmlCompilerOrchestrator>();

        hab.Services.AddSingleton<IAppLogger<AppLevel>, IndentedAppLogger<AppLevel>>(provider =>
        {
            //var settingsStore = provider.GetRequiredService<IAppOptionsStore>();

            var defaultLogLevels = new AppLoggerLevelStore<AppLevel>();
            var defaultCW = new ConsoleLogWriter();
            var defaultDependencies = new Dictionary<AppLevel, AppLevel>();

            return new IndentedAppLogger<AppLevel>(defaultLogLevels, defaultCW, dependencies: defaultDependencies);
        });

        using var tokenSource = new CancellationTokenSource();

        var host = hab.Build();

        var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
        Console.CancelKeyPress += (sender, e) =>
        {
            Console.WriteLine("Прервано по требованию");
            e.Cancel = true;
            lifetime.StopApplication();
            tokenSource.Cancel();
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

        await host.StartAsync();

        try
        {
            await mainService.RunAsync(lifetime.ApplicationStopping);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            lifetime.StopApplication();
            await host.WaitForShutdownAsync();
        }
    }
}

public class ContextInfoMapperFactory : IContextInfoMapperFactory
{
    private readonly IDictionary<MapperKeyBase, IContextKeyMap<ContextInfo, IContextKey>> _mappers;

    public ContextInfoMapperFactory(IDictionary<MapperKeyBase, IContextKeyMap<ContextInfo, IContextKey>> mappers)
    {
        _mappers = mappers;
    }

    public IContextKeyMap<ContextInfo, IContextKey> GetMapper(MapperKeyBase type)
    {
        if (_mappers.TryGetValue(type, out var mapper))
        {
            return mapper;
        }

        throw new NotImplementedException($"Mapper for type {type} is not registered.");
    }
}

public class ContextInfoFlatMapperFactory : IContextInfoIndexerFactory
{
    private readonly IDictionary<MapperKeyBase, IContextKeyIndexer<ContextInfo>> _mappers;

    public ContextInfoFlatMapperFactory(IDictionary<MapperKeyBase, IContextKeyIndexer<ContextInfo>> mappers)
    {
        _mappers = mappers;
    }

    public IContextKeyIndexer<ContextInfo> GetMapper(MapperKeyBase type)
    {
        if (_mappers.TryGetValue(type, out var mapper))
        {
            return mapper;
        }

        throw new NotImplementedException($"Mapper for type {type} is not registered.");
    }
}