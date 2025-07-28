using ContextBrowser.ContextKit.Model;
using ContextBrowser.DiagramFactory.Builders.TransitionDirectionBuilder;
using ContextBrowser.UmlKit.Diagrams;

namespace ContextBrowser.DiagramFactory.Builders;

public class ContextTransitionDiagramBuilder : IContextDiagramBuilder
{
    private readonly ContextTransitionDiagramBuilderOptions _options;
    private readonly List<ITransitionDirectionBuilder> _directionBuilders;
    private readonly List<string>? _filterDomains;
    private readonly UmlTransitionDtoBuilder _transitionBuilder;

    public string Name => "context-transition";

    public ContextTransitionDiagramBuilder(
        ContextTransitionDiagramBuilderOptions options,
        IEnumerable<ITransitionDirectionBuilder> directionBuilders,
        IControlParticipantResolver controlResolver,
        List<string>? filterDomains = null)
    {
        _options = options;
        _directionBuilders = directionBuilders.ToList();
        _filterDomains = filterDomains;
        _transitionBuilder = new UmlTransitionDtoBuilder(controlResolver);
    }

    public bool Build(string domainName, List<ContextInfo> allContexts, ContextClassifier classifier, UmlDiagram diagram)
    {
        var methods = allContexts
            .Where(ctx => ctx.ElementType == ContextInfoElementType.method &&
                          ctx.Domains.Contains(domainName) &&
                          classifier.HasActionAndDomain(ctx))
            .ToList();

        if(!methods.Any())
        {
            Console.WriteLine($"[MISS]: No methods for domain '{domainName}'");
            return false;
        }

        var allTransitions = new HashSet<UmlTransitionDto>();

        foreach(var builder in _directionBuilders)
        {
            var transitions = builder.BuildTransitions(methods);

            foreach(var t in transitions)
            {
                if(ShouldInclude(t))
                    allTransitions.Add(t);
            }
        }

        if(!allTransitions.Any())
            return false;

        foreach(var t in allTransitions)
        {
            TransitionRenderer.RenderTransition(t, diagram, _options);
        }

        return true;
    }

    private bool ShouldInclude(UmlTransitionDto dto)
    {
        if(_filterDomains == null)
            return true;

        return _filterDomains.Contains(dto.Domain) ||
               _filterDomains.Contains(dto.CallerName) ||
               _filterDomains.Contains(dto.CalleeName);
    }
}
