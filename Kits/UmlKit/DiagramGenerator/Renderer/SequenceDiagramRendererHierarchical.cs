using ContextBrowserKit.Log;
using UmlKit.Builders;
using UmlKit.Builders.Model;
using UmlKit.DiagramGenerator.Managers;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.DiagramGenerator.Renderer;

public class SequenceDiagramRendererHierarchical<P> : ISequenceDiagramRenderer<P>
    where P : IUmlParticipant
{
    private readonly IUmlTransitionFactory<P> _factory;
    private readonly DiagramBuilderOptions _options;
    private readonly OnWriteLog? _onWriteLog;

    public SequenceDiagramRendererHierarchical(OnWriteLog? onWriteLog, DiagramBuilderOptions options, IUmlTransitionFactory<P> factory)
    {
        _factory = factory;
        _options = options;
        _onWriteLog = onWriteLog;
    }

    // Этот метод будет публичным, и именно его вы будете вызывать
    public void Render(UmlDiagram<P> diagram, GrouppedSortedTransitionList? allTransitions)
    {
        var rootTransitions = allTransitions?.GetTransitionList();
        if (rootTransitions == null)
        {
            return;
        }
        var activationStack = new RenderContextActivationStack(_onWriteLog);

        RenderRecursive(diagram, rootTransitions, activationStack, allTransitions);
    }

    private IEnumerable<UmlTransitionDto> GetChildrenList(UmlTransitionDto? transition, GrouppedSortedTransitionList? allTransitions)
    {
        return Enumerable.Empty<UmlTransitionDto>();
    }

    private void RenderRecursive(UmlDiagram<P> diagram, List<UmlTransitionDto> transitions, RenderContextActivationStack activationStack, GrouppedSortedTransitionList? allTransitions)
    {
        foreach (var transition in transitions)
        {
            // Логика активации/деактивации на основе стека
            while (activationStack.Any() && activationStack.Peek() != transition.CallerClassName)
            {
                //RenderDeactivateCaller
                SequenceActivationManager.RenderDeactivateCallee(new RenderContext<P>(
                    transition, diagram, _options, activationStack, _onWriteLog));
            }

            if (!activationStack.Any() || activationStack.Peek() != transition.CallerClassName)
            {
                // Вызываем SystemCall для первого вызова в цепочке
                SequenceTransitionManager.SystemCall(_factory, _options, diagram,
                    transition.CallerClassName, transition.CallerMethod, true);

                activationStack.Push(transition.CallerClassName);
            }

            var defaultKeywords = new UmlParticipantKeywordsSet()
            {
                Actor = UmlParticipantKeyword.Actor,
                Control = UmlParticipantKeyword.Control,
            };

            // Логика рендеринга для текущего перехода
            var ctx = new RenderContext<P>(transition, diagram, _options, activationStack, _onWriteLog);
            SequenceParticipantsManager.AddParticipants(ctx, defaultKeywords);

            // Рендеринг вызовов
            SequenceActivationManager.RenderActivateCallee(ctx);
            SequenceInvocationManager.RenderCalleeInvocation(ctx);

            // Если есть дочерние вызовы, рекурсивно рендерим их
            var children = GetChildrenList(transition, allTransitions);
            if (children.Any())
            {
                RenderRecursive(diagram, children.ToList(), activationStack, allTransitions);
            }

            // Логика деактивации после возврата
            SequenceActivationManager.RenderDeactivateCallee(ctx);
        }
    }
}