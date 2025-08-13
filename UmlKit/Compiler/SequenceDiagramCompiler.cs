using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using UmlKit.Builders;
using UmlKit.Builders.TransitionFactory;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;
using UmlKit.Renderer;

namespace ContextBrowser.Infrastructure.Compiler;

/// <summary>
/// Класс для компиляции диаграмм последовательности на основе контекстной информации.
/// </summary>
public class SequenceDiagramCompiler
{
    // Свойства класса, инициализируемые в конструкторе
    private readonly IContextClassifier _classifier;
    private readonly string _outputPath;
    private readonly OnWriteLog? _onWriteLog;
    private readonly DiagramBuilderOptions _options;
    private readonly IContextDiagramBuilder _diagramBuilder;

    /// <summary>
    /// Создает новый экземпляр компилятора.
    /// </summary>
    /// <param name="classifier">Классификатор контекста.</param>
    /// <param name="outputPath">Путь для сохранения выходных файлов.</param>
    /// <param name="onWriteLog">Делегат для записи логов.</param>
    /// <param name="options">Опции для построителя диаграммы переходов.</param>
    /// <param name="diagramBuilder">Построителя диаграмм.</param>
    public SequenceDiagramCompiler(
        IContextClassifier classifier,
        string outputPath,
        OnWriteLog? onWriteLog,
        DiagramBuilderOptions options,
        IContextDiagramBuilder diagramBuilder)
    {
        _classifier = classifier;
        _outputPath = outputPath;
        _onWriteLog = onWriteLog;
        _options = options;
        _diagramBuilder = diagramBuilder;
    }

    /// <summary>
    /// Компилирует и рендерит диаграмму последовательности для заданного домена и контекстных элементов.
    /// </summary>
    /// <param name="domain">Имя домена.</param>
    /// <param name="contextItems">Список контекстных элементов.</param>
    /// <returns>Возвращает true, если рендеринг был успешным, иначе false.</returns>
    public bool Compile(string domain, List<ContextInfo> contextItems)
    {
        _onWriteLog?.Invoke(AppLevel.P_Cpl, LogLevel.Dbg, $"Compile sequence for [{domain}]", LogLevelNode.Start);


        // Создаем новую диаграмму UML-последовательности
        var diagram = new UmlDiagramSequence(_options);
        diagram.SetTitle($"Domain: {domain}");
        diagram.SetSkinParam("componentStyle", "rectangle");

        // Используем фабрику для создания построителя диаграмм
        var builder = _diagramBuilder;
        var transitions = builder.Build(domain, contextItems, _classifier);

        _onWriteLog?.Invoke(AppLevel.P_Cpl, LogLevel.Dbg, $"Render domain [{domain}]", LogLevelNode.Start);

        // Используем рендерер для отрисовки переходов
        var _factory = new UmlTransitionParticipantFactory();
        var _renderer = new SequenceRenderer<UmlParticipant>(_options, _onWriteLog, _factory);
        var result = _renderer.RenderDiagramTransitions(diagram, transitions, domain);

        if (result)
        {
            // Если рендеринг успешен, записываем диаграмму в файл
            var path = Path.Combine(_outputPath, $"sequence_domain_{domain}.puml");
            diagram.WriteToFile(path);
        }

        _onWriteLog?.Invoke(AppLevel.P_Cpl, LogLevel.Dbg, string.Empty, LogLevelNode.End);

        _onWriteLog?.Invoke(AppLevel.P_Cpl, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        return result;
    }
}
