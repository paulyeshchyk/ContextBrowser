using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.ContextData.Naming;
using ContextKit.Model;
using LoggerKit;
using UmlKit;
using UmlKit.Builders;
using UmlKit.Builders.TransitionFactory;
using UmlKit.DiagramGenerator;
using UmlKit.DiagramGenerator.Renderer;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;

namespace ExporterKit.Uml.DiagramCompiler.CoCompiler.Sequence;

/// <summary>
/// Класс для компиляции диаграмм последовательности на основе контекстной информации.
/// </summary>
public class UmlDiagramCompilerSequence
{
    // Свойства класса, инициализируемые в конструкторе
    private readonly ExportOptions _exportOptions;
    private readonly IAppLogger<AppLevel> _logger;
    private readonly DiagramBuilderOptions _options;
    private readonly IContextDiagramBuilder _diagramBuilder;
    private readonly INamingProcessor _namingProcessor;
    private readonly UmlTransitionRendererFlat<UmlParticipant> _renderer;

    /// <summary>
    /// Создает новый экземпляр компилятора.
    /// </summary>
    /// <param name="logger">Делегат для записи логов.</param>
    /// <param name="exportOptions">Путь для сохранения выходных файлов.</param>
    /// <param name="options">Опции для построителя диаграммы переходов.</param>
    /// <param name="diagramBuilder">Построитель диаграмм.</param>
    /// <param name="namingProcessor"></param>
    public UmlDiagramCompilerSequence(
        IAppLogger<AppLevel> logger,
        ExportOptions exportOptions,
        DiagramBuilderOptions options,
        IContextDiagramBuilder diagramBuilder,
        INamingProcessor namingProcessor,
        UmlTransitionRendererFlat<UmlParticipant> renderer)
    {
        _exportOptions = exportOptions;
        _logger = logger;
        _options = options;
        _diagramBuilder = diagramBuilder;
        _namingProcessor = namingProcessor;
        _renderer = renderer;
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
    /// <param name="cancellationToken"></param>
    /// <returns>Возвращает true, если рендеринг был успешным, иначе false.</returns>
    public async Task<bool> CompileAsync(string metaItem, FetchType fetchType, string diagramId, string title, string outputFileName, List<ContextInfo> contextItems, CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Dbg, $"Compile sequence for [{metaItem}]", LogLevelNode.Start);

        var transitions = _diagramBuilder.Build(metaItem, fetchType, contextItems);

        if (transitions == null || !transitions.HasTransitions())
        {
            _logger.WriteLog(AppLevel.P_Rnd, LogLevel.Warn, $"No transitions provided for [{metaItem}]");
            return false;
        }

        _logger.WriteLog(AppLevel.P_Rnd, LogLevel.Dbg, $"Rendering Diagram transitions for [{metaItem}]", LogLevelNode.Start);

        var diagram = await _renderer.RenderAsync(transitions, cancellationToken).ConfigureAwait(false);

        _logger.WriteLog(AppLevel.P_Rnd, LogLevel.Dbg, string.Empty, LogLevelNode.End);

        if (diagram == null)
        {
            _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Dbg, string.Empty, LogLevelNode.End);
            return false;
        }

        diagram.DiagramId = diagramId;
        diagram.SetTitle(title);
        diagram.SetSkinParam("componentStyle", "rectangle");

        var writeOptons = new UmlWriteOptions(alignMaxWidth: -1);
        var path = _exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, outputFileName);
        await diagram.WriteToFileAsync(path, writeOptons, cancellationToken).ConfigureAwait(false);

        return true;
    }
}
