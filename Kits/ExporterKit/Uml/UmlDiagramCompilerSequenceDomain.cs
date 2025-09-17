﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit;
using ExporterKit.Html;
using ExporterKit.Infrastucture;
using ExporterKit.Uml;
using ExporterKit.Uml.DiagramCompileOptions;
using LoggerKit;
using UmlKit.Builders;
using UmlKit.Compiler;
using UmlKit.Compiler.CoCompiler;
using UmlKit.Infrastructure.Options;

namespace ExporterKit.Uml;

// context: uml, sequence, build
public class UmlDiagramCompilerSequenceDomain : IUmlDiagramCompiler
{
    protected readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider _datasetProvider;
    private readonly IContextInfoMapperProvider _mapperProvider;
    private readonly IAppOptionsStore _optionsStore;

    public UmlDiagramCompilerSequenceDomain(IAppLogger<AppLevel> logger, IContextInfoDatasetProvider datasetProvider, IContextInfoMapperProvider mapperProvider, IAppOptionsStore optionsStore)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _mapperProvider = mapperProvider;
        _optionsStore = optionsStore;
    }

    // context: uml, build
    public async Task<Dictionary<string, bool>> CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, "Compile SequenceDomain");

        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);

        var contextClassifier = _optionsStore.GetOptions<IDomainPerActionContextClassifier>();
        var exportOptions = _optionsStore.GetOptions<ExportOptions>();
        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();

        var elements = dataset.GetAll().ToList();
        var mapper = await _mapperProvider.GetMapperAsync(GlobalMapperKeys.DomainPerAction, cancellationToken);
        var distinctCols = mapper.GetCols().Distinct();

        var renderedCache = new Dictionary<string, bool>();
        foreach (var col in distinctCols)
        {
            var compileOptions = DiagramCompileOptionsFactory.DomainSequenceCompileOptions(col);
            renderedCache[col] = GenerateSingle(contextClassifier, exportOptions, compileOptions, diagramBuilderOptions, elements);
        }
        return renderedCache;
    }

    /// <summary>
    /// Компилирует диаграмму последовательностей.
    /// </summary>
    protected bool GenerateSingle(IDomainPerActionContextClassifier contextClassifier, ExportOptions exportOptions, IDiagramCompileOptions options, DiagramBuilderOptions diagramBuilderOptions, List<ContextInfo> allContexts)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, $"Compiling Sequence {options.FetchType} [{options.MetaItem}]", LogLevelNode.Start);
        var bf = ContextDiagramBuildersFactory.TransitionBuilder(diagramBuilderOptions, _logger.WriteLog);

        var diagramCompilerSequence = new UmlDiagramCompilerSequence(logger: _logger, classifier: contextClassifier, exportOptions, diagramBuilderOptions, bf);
        var rendered = diagramCompilerSequence.Compile(options.MetaItem, options.FetchType, options.DiagramId, options.DiagramTitle, options.OutputFileName, allContexts, CancellationToken.None);

        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, string.Empty, LogLevelNode.End);
        return rendered;
    }
}
