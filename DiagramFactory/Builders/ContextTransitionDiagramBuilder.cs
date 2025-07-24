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
            onGetDescendants: m => m.References?.Where(r => r.ElementType == ContextInfoElementType.method) ?? Enumerable.Empty<ContextInfo>(),
            onGetDomainItems: d =>
                allContexts.Where(c =>
                    c.Contexts.Contains(d) &&
                    classifier.HasActionAndDomain(c)),
            onExportItem:(caller, callee, calleeDomainItem, domain) =>
            {
                var callerLabel = $"{caller.ClassOwner?.Name}.{caller.Name}".Replace(".", "_");
                var calleeLabel = $"{callee.ClassOwner?.Name}.{callee.Name}".Replace(".", "_");
                var domainLabel = domain.Replace(".", "_");

                target.AddState(domainLabel);
                target.AddState(callerLabel);
                target.AddState(calleeLabel);

                target.AddTransition(domainLabel, callerLabel, "entry");
                target.AddTransition(callerLabel, calleeLabel, "calls");
                target.AddTransition(calleeLabel, domainLabel, "exit");
            }
        );

        walker.Walk(filtered);
        return true;
    }
}