using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options;
using ContextKit.ContextData.Naming;
using LoggerKit;
using UmlKit.Builders;
using UmlKit.Builders.Model;
using UmlKit.DataProviders;
using UmlKit.DiagramGenerator.Managers;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.DiagramGenerator.Renderer;

public abstract class UmlTransitionRendererHierarchial<P> : IUmlTransitionRenderer<P>
    where P : IUmlParticipant
{
    private readonly IUmlTransitionFactory<P> _factory;
    private readonly DiagramBuilderOptions _options;
    private readonly IAppLogger<AppLevel> _logger;
    private readonly INamingProcessor _namingProcessor;

    public UmlTransitionRendererHierarchial(IAppLogger<AppLevel> logger, DiagramBuilderOptions options, IUmlTransitionFactory<P> factory, INamingProcessor namingProcessor)
    {
        _factory = factory;
        _options = options;
        _logger = logger;
        _namingProcessor = namingProcessor;
    }

    public abstract UmlDiagram<P> CreateDiagram();

    public Task<UmlDiagram<P>?> RenderAsync(GrouppedSortedTransitionList? allTransitions, CancellationToken cancellationToken)
    {
        UmlDiagram<P>? diagram = null;
        var rootTransitions = allTransitions?.GetTransitionList();
        if (rootTransitions == null)
        {
            return Task.FromResult(diagram);
        }

        diagram = CreateDiagram();

        var activationStack = new RenderContextActivationStack(_logger);

        RenderRecursive(diagram, rootTransitions, activationStack, allTransitions);
        return Task.FromResult((UmlDiagram<P>?)diagram);
    }

    private IEnumerable<UmlTransitionDto> GetChildrenList(UmlTransitionDto? transition, GrouppedSortedTransitionList? allTransitions)
    {
        return [];
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
                    transition, diagram, _options, activationStack, _logger));
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
            var ctx = new RenderContext<P>(transition, diagram, _options, activationStack, _logger);
            SequenceParticipantsManager.AddParticipants(ctx, defaultKeywords, _namingProcessor);

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