using ContextBrowser.ContextKit.Model;
using ContextBrowser.ContextKit.Parser;
using ContextBrowser.UmlKit.Diagrams;

namespace ContextBrowser.DiagramFactory.Builders;

public class ContextTransitionDiagramBuilder : IContextDiagramBuilder
{
    public string Name => "context-transition";

    public bool Build(string domainName, List<ContextInfo> allContexts, ContextClassifier classifier, UmlDiagram target)
    {
        var methodsInDomain = allContexts
            .Where(ctx =>
                ctx.ElementType == ContextInfoElementType.method &&
                ctx.Domains.Contains(domainName) &&
                classifier.HasActionAndDomain(ctx))
            .ToList();

        if(!methodsInDomain.Any())
            return false;

        var transitions = new HashSet<UmlTransitionDto>();

        foreach(var caller in methodsInDomain)
        {
            foreach(var callee in caller.References)
            {
                if(callee.ElementType != ContextInfoElementType.method)
                    continue;

                var calleeDomain = callee.Domains.FirstOrDefault();
                if(string.IsNullOrWhiteSpace(calleeDomain))
                    continue;

                transitions.Add(new UmlTransitionDto(caller.DisplayName, callee.DisplayName, calleeDomain));
            }
        }

        foreach(var t in transitions)
        {
            target.AddParticipant(t.domain);
            target.AddParticipant(t.caller);
            target.AddParticipant(t.callee);

            target.AddTransition(t.domain, t.caller, "entry");
            target.AddTransition(t.caller, t.callee, "calls");
            target.AddTransition(t.callee, t.domain, "exit");
        }


        return transitions.Count > 0;
    }
}

public record struct UmlTransitionDto(string caller, string callee, string domain)
{
    public static implicit operator (string caller, string callee, string domain)(UmlTransitionDto value)
    {
        return (value.caller, value.callee, value.domain);
    }

    public static implicit operator UmlTransitionDto((string caller, string callee, string domain) value)
    {
        return new UmlTransitionDto(value.caller, value.callee, value.domain);
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(caller, callee, domain);
    }
}