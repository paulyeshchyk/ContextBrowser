using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using UmlKit.Builders;
using UmlKit.Builders.Model;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.Renderer.Sequence;

public class SequenceDiagramGenerator<P>
        where P : IUmlParticipant
{
    private readonly OnWriteLog? _onWriteLog;
    private readonly DiagramBuilderOptions _options;
    private readonly IUmlTransitionFactory<P> _factory;
    private readonly SequenceTransitionProcessor<P> _processor;

    public SequenceDiagramGenerator(DiagramBuilderOptions options, OnWriteLog? onWriteLog, IUmlTransitionFactory<P> factory)
    {
        _options = options;
        _onWriteLog = onWriteLog;
        _factory = factory;
        _processor = new SequenceTransitionProcessor<P>(options, onWriteLog, factory);
    }

    public bool RenderDiagramTransitions(UmlDiagram<P> diagram, GrouppedSortedTransitionList? allTransitions, string domain)
    {
        if(allTransitions == null || !allTransitions.HasTransitions())
        {
            _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Warn, $"No transitions provided for [{domain}]");
            return false;
        }

        _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"Rendering Diagram transitions for [{domain}]", LogLevelNode.Start);

        var callStack = new Stack<string>();
        var activationStack = new RenderContextActivationStack(_onWriteLog);

        foreach(var transition in allTransitions.GetTransitionList())
        {
            var caller = transition.CallerClassName;
            var callee = transition.CalleeClassName;

            // --- 1. Обработка возврата из вложенных вызовов ---
            // Если текущий вызывающий не совпадает с вершиной стека,
            // это означает, что мы выходим из одного или нескольких вложенных вызовов.
            while(callStack.Any() && callStack.Peek() != caller)
            {
                diagram.Deactivate(callStack.Pop());
            }

            // --- 2. Установка нового контекста или продолжение текущего ---
            // Если стек пуст или вершина стека не является текущим вызывающим,
            // это новый вызов. Добавляем его в стек.
            if(!callStack.Any() || callStack.Peek() != caller)
            {
                SequenceRendererUtils.SystemCall(_factory, _options, diagram, caller, transition.CallerMethod ?? "_unknown_caller_method", true);
                callStack.Push(caller);
            }

            // --- 3. Рендеринг текущего перехода ---
            _processor.RenderSingleTransition(new RenderContext<P>(transition, diagram, _options, activationStack, _onWriteLog));

            // --- 4. Добавление нового вызываемого объекта в стек ---
            if(!string.IsNullOrWhiteSpace(callee) && !callStack.Contains(callee))
            {
                callStack.Push(callee);
            }
        }

        // --- Закрытие оставшихся контекстов ---
        while(callStack.Count > 0)
        {
            diagram.Deactivate(callStack.Pop());
        }

        _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        return true;
    }
}

