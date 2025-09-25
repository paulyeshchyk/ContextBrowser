using System.Collections.Generic;
using System.Threading;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Classifier;
using LoggerKit;
using UmlKit.Builders;
using UmlKit.Builders.TransitionFactory;
using UmlKit.DiagramGenerator;
using UmlKit.DiagramGenerator.Renderer;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.Compiler.CoCompiler;

/// <summary>
/// Класс для компиляции диаграмм последовательности на основе контекстной информации.
/// </summary>
public class UmlDiagramCompilerSequence
{
    // Свойства класса, инициализируемые в конструкторе
    private readonly ITensorClassifierDomainPerActionContext _classifier;
    private readonly ExportOptions _exportOptions;
    private readonly IAppLogger<AppLevel> _logger;
    private readonly DiagramBuilderOptions _options;
    private readonly IContextDiagramBuilder _diagramBuilder;

    /// <summary>
    /// Создает новый экземпляр компилятора.
    /// </summary>
    /// <param name="classifier">Классификатор контекста.</param>
    /// <param name="exportOptions">Путь для сохранения выходных файлов.</param>
    /// <param name="onWriteLog">Делегат для записи логов.</param>
    /// <param name="options">Опции для построителя диаграммы переходов.</param>
    /// <param name="diagramBuilder">Построителя диаграмм.</param>
    public UmlDiagramCompilerSequence(
        IAppLogger<AppLevel> logger,
        ITensorClassifierDomainPerActionContext classifier,
        ExportOptions exportOptions,
        DiagramBuilderOptions options,
        IContextDiagramBuilder diagramBuilder)
    {
        _classifier = classifier;
        _exportOptions = exportOptions;
        _logger = logger;
        _options = options;
        _diagramBuilder = diagramBuilder;
    }

    /// <summary>
    /// Компилирует и рендерит диаграмму последовательности для заданного элемента метаданных.
    /// </summary>
    /// <param name="metaItem">Элемент метаданных.</param>
    /// <param name="fetchType"></param>
    /// <param name="diagramId">Идентификатор диаграммы.</param>
    /// <param name="title">Заголовок диаграммы.</param>
    /// <param name="outputFileName">Имя выходного файла.</param>
    /// <param name="contextItems">Список контекстных элементов.</param>
    /// <returns>Возвращает true, если рендеринг был успешным, иначе false.</returns>
    public bool Compile(string metaItem, FetchType fetchType, string diagramId, string title, string outputFileName, List<ContextInfo> contextItems, CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Dbg, $"Compile sequence for [{metaItem}]", LogLevelNode.Start);

        var diagram = new UmlDiagramSequence(_options, diagramId: diagramId);
        diagram.SetTitle(title);
        diagram.SetSkinParam("componentStyle", "rectangle");

        var transitions = _diagramBuilder.Build(metaItem, fetchType, contextItems);

        var factory = new UmlTransitionParticipantFactory();
        var renderer = new SequenceDiagramRendererPlain<UmlParticipant>(_logger, _options, factory);
        var generator = new SequenceDiagramGenerator<UmlParticipant>(renderer, _options, _logger, factory);
        var result = generator.Generate(diagram, transitions, metaItem);

        if (result)
        {
            var writeOptons = new UmlWriteOptions(alignMaxWidth: -1) { };
            var path = _exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, outputFileName);
            diagram.WriteToFile(path, writeOptons);
        }

        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        return result;
    }
}
