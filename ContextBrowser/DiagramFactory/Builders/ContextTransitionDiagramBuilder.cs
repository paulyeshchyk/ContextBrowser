using ContextBrowser.ContextKit.Model;
using ContextBrowser.UmlKit.Diagrams;
using ContextBrowser.UmlKit.Model;

namespace ContextBrowser.DiagramFactory.Builders;

public class ContextTransitionDiagramBuilder : IContextDiagramBuilder
{
    private readonly ContextTransitionDiagramBuilderOptions _options;
    private readonly List<string>? _filterDomains;
    private readonly UmlTransitionDtoBuilder _transitionBuilder;

    public string Name => "context-transition";

    public ContextTransitionDiagramBuilder(
        ContextTransitionDiagramBuilderOptions options,
        IControlParticipantResolver controlResolver,
        List<string>? filterDomains = null)
    {
        _options = options;
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

        var transitions = new HashSet<UmlTransitionDto>();

        foreach(var ctx in methods)
        {
            foreach(var callee in ctx.References)
            {
                if(callee.ElementType != ContextInfoElementType.method)
                    continue;

                if(!ShouldInclude(ctx, callee))
                    continue;

                transitions.Add(_transitionBuilder.CreateTransition(ctx, callee));
            }
        }

        if(_options.Direction is DiagramDirection.Incoming or DiagramDirection.BiDirectional)
        {
            foreach(var ctx in methods)
            {
                foreach(var caller in ctx.InvokedBy ?? Enumerable.Empty<ContextInfo>())
                {
                    if(caller.ElementType != ContextInfoElementType.method)
                        continue;

                    if(!ShouldInclude(caller, ctx))
                        continue;

                    transitions.Add(_transitionBuilder.CreateTransition(caller, ctx));
                }
            }
        }

        if(!transitions.Any())
            return false;

        foreach(var t in transitions)
        {
            TransitionRenderer.RenderTransition(t, diagram, _options);
        }

        return true;
    }

    private bool ShouldInclude(ContextInfo caller, ContextInfo callee)
    {
        if(_filterDomains == null)
            return true;

        bool callerMatch = caller.Contexts.Overlaps(_filterDomains);
        bool calleeMatch = callee.Contexts.Overlaps(_filterDomains);
        return callerMatch || calleeMatch;
    }
}

internal class UmlTransitionDtoBuilder
{
    private readonly IControlParticipantResolver _controlResolver;

    public UmlTransitionDtoBuilder(IControlParticipantResolver controlResolver)
    {
        _controlResolver = controlResolver;
    }

    public UmlTransitionDto CreateTransition(ContextInfo caller, ContextInfo callee)
    {
        return new UmlTransitionDto
        {
            CallerId = caller.PlantUmlId,
            CalleeId = callee.PlantUmlId,
            CallerName = caller.ClassOwner?.Name ?? caller.Name,
            CalleeName = callee.ClassOwner?.Name ?? callee.Name,
            CallerMethod = caller.Name,
            CalleeMethod = callee.Name,
            Domain = callee.Domains.FirstOrDefault() ?? "unknown",
            RunContext = callee.MethodOwner?.ClassOwner?.Name
        };
    }
}

public interface IControlParticipantResolver
{
    bool TryGetControl(ContextInfo caller, out string controlName);
}

public class RunMethodAsControlParticipantResolver : IControlParticipantResolver
{
    public bool TryGetControl(ContextInfo caller, out string controlName)
    {
        controlName = default!;
        if(caller.MethodOwner?.Name == "Run" && caller.ClassOwner?.Name is { } owner)
        {
            controlName = owner;
            return true;
        }

        return false;
    }
}

internal static class TransitionRenderer
{
    public static void RenderTransition(UmlTransitionDto t, UmlDiagram diagram, ContextTransitionDiagramBuilderOptions builderOptions)
    {
        var callerPart = builderOptions.DetailLevel == DiagramDetailLevel.Summary ? t.CallerName : t.CallerId;
        var calleePart = builderOptions.DetailLevel == DiagramDetailLevel.Summary ? t.CalleeName : t.CalleeId;

        diagram.AddParticipant(t.Domain);
        diagram.AddParticipant(callerPart, builderOptions.DefaultParticipantKeyword);
        diagram.AddParticipant(calleePart, builderOptions.DefaultParticipantKeyword);

        switch(builderOptions.DetailLevel)
        {
            case DiagramDetailLevel.Summary:
                RenderSummaryTransition(diagram, t);
                break;
            case DiagramDetailLevel.Method:
                RenderMethodTransition(diagram, t);
                break;
            case DiagramDetailLevel.Full:
                RenderFullTransition(diagram, t);
                break;
        }
    }

    private static void RenderSummaryTransition(UmlDiagram diagram, UmlTransitionDto t)
    {
        diagram.AddTransition(t.Domain, t.CallerName, "entry");
        diagram.AddTransition(t.CallerName, t.CalleeName, "calls");
        diagram.AddTransition(t.CalleeName, t.Domain, "exit");
    }

    private static void RenderMethodTransition(UmlDiagram diagram, UmlTransitionDto t)
    {
        diagram.AddTransition(t.Domain, t.CallerName, t.CallerMethod);
        diagram.AddTransition(t.CallerName, t.CalleeName, t.CalleeMethod);
        diagram.AddTransition(t.CalleeName, t.Domain, "done");
    }

    private static void RenderFullTransition(UmlDiagram diagram, UmlTransitionDto t)
    {
        if(!string.IsNullOrWhiteSpace(t.RunContext))
        {
            diagram.AddParticipant(t.RunContext, UmlParticipantKeyword.Control);
            diagram.AddTransition(t.CallerName, t.RunContext, t.CallerMethod);
            diagram.AddTransition(t.RunContext, t.CalleeName, t.CalleeMethod);
            diagram.AddTransition(t.CalleeName, t.RunContext, "return");
            diagram.AddTransition(t.RunContext, t.CallerName, "done");
        }
        else
        {
            diagram.AddTransition(t.CallerName, t.CalleeName, t.CalleeMethod);
            diagram.AddTransition(t.CalleeName, t.CallerName, "done");
        }
    }
}

public record ContextTransitionDiagramBuilderOptions
{
    public DiagramDetailLevel DetailLevel = DiagramDetailLevel.Full;

    public DiagramDirection Direction = DiagramDirection.BiDirectional;

    public UmlParticipantKeyword DefaultParticipantKeyword = UmlParticipantKeyword.Actor;
}

public enum DiagramDetailLevel
{
    Summary,    // Показываем только взаимодействия между контекстами
    Method,     // Показываем имена вызываемых методов
    Full        // Показываем "Run()", возвраты, возможно параметры
}
