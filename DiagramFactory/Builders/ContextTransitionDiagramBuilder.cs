using ContextBrowser.ContextKit.graph;
using ContextBrowser.ContextKit.Model;
using ContextBrowser.ContextKit.Parser;
using ContextBrowser.UmlKit.Diagrams;

namespace ContextBrowser.DiagramFactory.Builders;

public class ContextTransitionDiagramBuilder : IContextDiagramBuilder
{
    public string Name => "context-transition";

    public bool Build(string domainName, List<ContextInfo> allContexts, ContextClassifier classifier, UmlDiagram target)
    {
        var filtered = allContexts
            .Where(ctx =>
                ctx.Domains.Contains(domainName) &&
                classifier.HasActionAndDomain(ctx))
            .ToList();

        if(!filtered.Any())
            return false;

        var walker = new ItemWalker(
            onGetDescendants: method =>
            {
                return method.References ?? Enumerable.Empty<ContextInfo>();
            },
            onGetDomainItems: d =>
            {
                return allContexts.Where(c => c.Contexts.Contains(d) && classifier.HasActionAndDomain(c));
            },
            onExportItem:(caller, callee, calleeDomainItem, domain) =>
            {
                var callerLabel = $"{caller.ClassOwner?.Name}.{caller.Name}";
                var calleeLabel = $"{callee.ClassOwner?.Name}.{callee.Name}";
                var domainLabel = domain;

                target.AddParticipant(domainLabel);
                target.AddParticipant(callerLabel);
                target.AddParticipant(calleeLabel);

                target.AddTransition(domainLabel, callerLabel, "entry");
                target.AddTransition(callerLabel, calleeLabel, "calls");
                target.AddTransition(calleeLabel, domainLabel, "exit");
            }
        );

        walker.Walk(filtered);
        return true;
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