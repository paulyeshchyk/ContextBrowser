using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using UmlKit.Builders;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;
using UmlKit.Renderer;

namespace ContextBrowser.Infrastructure.Compiler;

public class StateDiagramCompiler
{
    // Свойства класса, инициализируемые в конструкторе
    private readonly IContextClassifier _classifier;
    private readonly DiagramBuilderOptions _options;
    private readonly IContextDiagramBuilder _builder;
    private readonly string _outputDirectory;
    private readonly SequenceRenderer<UmlState> _renderer;
    private readonly OnWriteLog? _onWriteLog;

    /// <summary>
    /// Создает новый экземпляр компилятора.
    /// </summary>
    /// <param name="classifier">Классификатор контекста.</param>
    /// <param name="options">Опции для построителя диаграммы переходов.</param>
    /// <param name="builder">Фабрика для создания построителя диаграмм.</param>
    /// <param name="outputDirectory">Каталог для сохранения выходных файлов.</param>
    /// <param name="renderer">Рендерер переходов.</param>
    /// <param name="onWriteLog">Делегат для записи логов.</param>
    public StateDiagramCompiler(
        IContextClassifier classifier,
        DiagramBuilderOptions options,
        IContextDiagramBuilder builder,
        string outputDirectory,
        SequenceRenderer<UmlState> renderer,
        OnWriteLog? onWriteLog)
    {
        _classifier = classifier;
        _options = options;
        _builder = builder;
        _outputDirectory = outputDirectory;
        _renderer = renderer;
        _onWriteLog = onWriteLog;
    }

    /// <summary>
    /// Компилирует и рендерит диаграмму состояний для заданного домена и всех контекстов.
    /// </summary>
    /// <param name="domain">Имя домена.</param>
    /// <param name="allContexts">Список всех контекстных элементов.</param>
    public bool Compile(string domain, List<ContextInfo> allContexts)
    {
        _onWriteLog?.Invoke(AppLevel.P_Cpl, LogLevel.Dbg, $"Compile state for [{domain}]", LogLevelNode.Start);

        // Создаем новую диаграмму состояний
        var diagram = new UmlDiagramState(_options);
        diagram.SetTitle($"Context: {domain}");
        diagram.SetSkinParam("componentStyle", "rectangle");

        // Используем фабрику для создания построителя диаграмм
        var transitions = _builder.Build(domain, allContexts, _classifier);

        _onWriteLog?.Invoke(AppLevel.P_Cpl, LogLevel.Dbg, $"Render domain [{domain}]", LogLevelNode.Start);
        var rendered = _renderer.RenderDiagramTransitions(diagram, transitions, domain);

        if (rendered)
        {
            // Если рендеринг успешен, записываем диаграмму в файл
            var path = Path.Combine(_outputDirectory, $"state_domain_{domain}.puml");
            diagram.WriteToFile(path);
        }
        _onWriteLog?.Invoke(AppLevel.P_Cpl, LogLevel.Dbg, string.Empty, LogLevelNode.End);

        _onWriteLog?.Invoke(AppLevel.P_Cpl, LogLevel.Dbg, string.Empty, LogLevelNode.End);

        return true;
    }
}