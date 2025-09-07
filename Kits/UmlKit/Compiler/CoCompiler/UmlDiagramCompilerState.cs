using System.Collections.Generic;
using ContextBrowser;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using UmlKit.Builders;
using UmlKit.Compiler.CoCompiler;
using UmlKit.DiagramGenerator;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.Compiler.CoCompiler;

public class UmlStateDiagramCompilerAction : UmlDiagramCompilerState
{
    protected override FetchType _fetchType => FetchType.FetchAction;

    public UmlStateDiagramCompilerAction(IContextClassifier classifier, DiagramBuilderOptions options, IContextDiagramBuilder builder, ExportOptions exportOptions, SequenceDiagramGenerator<UmlState> renderer, OnWriteLog? onWriteLog) : base(classifier, options, builder, exportOptions, renderer, onWriteLog)
    {
    }
}

public class UmlStateDiagramCompilerDomain : UmlDiagramCompilerState
{
    protected override FetchType _fetchType => FetchType.FetchDomain;

    public UmlStateDiagramCompilerDomain(IContextClassifier classifier, DiagramBuilderOptions options, IContextDiagramBuilder builder, ExportOptions exportOptions, SequenceDiagramGenerator<UmlState> renderer, OnWriteLog? onWriteLog) : base(classifier, options, builder, exportOptions, renderer, onWriteLog)
    {
    }
}

public abstract class UmlDiagramCompilerState
{
    // Свойства класса, инициализируемые в конструкторе
    private readonly IContextClassifier _classifier;
    private readonly DiagramBuilderOptions _options;
    private readonly IContextDiagramBuilder _builder;
    private readonly ExportOptions _exportOptions;
    private readonly SequenceDiagramGenerator<UmlState> _renderer;
    private readonly OnWriteLog? _onWriteLog;

    protected abstract FetchType _fetchType { get; }

    /// <summary>
    /// Создает новый экземпляр компилятора.
    /// </summary>
    /// <param name="classifier">Классификатор контекста.</param>
    /// <param name="options">Опции для построителя диаграммы переходов.</param>
    /// <param name="builder">Фабрика для создания построителя диаграмм.</param>
    /// <param name="exportOptions">Каталог для сохранения выходных файлов.</param>
    /// <param name="renderer">Рендерер переходов.</param>
    /// <param name="onWriteLog">Делегат для записи логов.</param>
    public UmlDiagramCompilerState(
        IContextClassifier classifier,
        DiagramBuilderOptions options,
        IContextDiagramBuilder builder,
        ExportOptions exportOptions,
        SequenceDiagramGenerator<UmlState> renderer,
        OnWriteLog? onWriteLog)
    {
        _classifier = classifier;
        _options = options;
        _builder = builder;
        _exportOptions = exportOptions;
        _renderer = renderer;
        _onWriteLog = onWriteLog;
    }

    /// <summary>
    /// Компилирует и рендерит диаграмму состояний для заданного домена и всех контекстов.
    /// </summary>
    /// <param name="metaItem">Имя домена.</param>
    /// <param name="allContexts">Список всех контекстных элементов.</param>
    public bool Compile(string metaItem, string diagramId, string diagramFileName, List<ContextInfo> allContexts)
    {
        _onWriteLog?.Invoke(AppLevel.P_Cpl, LogLevel.Dbg, $"Compile state for [{metaItem}]", LogLevelNode.Start);

        var diagram = new UmlDiagramState(_options, diagramId: diagramId);
        diagram.SetTitle($"Context: {metaItem}");
        diagram.SetSkinParam("componentStyle", "rectangle");
        diagram.SetLayoutDirection(UmlLayoutDirection.Direction.LeftToRight);

        // Используем фабрику для создания построителя диаграмм
        var transitions = _builder.Build(metaItem, fetchType: _fetchType, allContexts, _classifier);

        var rendered = _renderer.Generate(diagram, transitions, metaItem);

        if (rendered)
        {
            // Если рендеринг успешен, записываем диаграмму в файл
            var path = _exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, diagramFileName);
            diagram.WriteToFile(path, -1);
        }

        _onWriteLog?.Invoke(AppLevel.P_Cpl, LogLevel.Dbg, string.Empty, LogLevelNode.End);

        return true;
    }
}