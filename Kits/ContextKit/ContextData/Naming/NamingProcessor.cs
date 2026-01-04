using ContextBrowserKit.Extensions;

namespace ContextKit.ContextData.Naming;

public class NamingProcessor : INamingProcessor
{
    public string ClassActionPumlFilename(string action) => string.Format("class_action_{0}.puml", action.AlphanumericOnly());
    public string ClassActionDiagramId(string action) => string.Format("class_action_{0}", action.AlphanumericOnly());

    public string ClassDomainPumlFilename(string domain) => string.Format("class_domain_{0}.puml", domain.AlphanumericOnly());
    public string ClassDomainDiagramId(string domain) => string.Format("class_domain_{0}", domain.AlphanumericOnly());

    public string ClassActionDomainDiagramId(string action, string domain) => string.Format("class_{0}_{1}", action, domain);
    public string ClassActionDomainPumlFilename(string action, string domain) => string.Format("class_{0}_{1}.puml", action, domain);
    public string ClassActionDomainDiagramTitle(string action, string domain) => string.Format("{0} -> {1}", action, domain);

    public string ClassOnlyHtmlFilename(string? className) => string.Format("class_only_{0}.html", className?.AlphanumericOnly());
    public string ClassOnlyPumlFilename(string? className) => string.Format("class_only_{0}.puml", className?.AlphanumericOnly());
    public string ClassOnlyDiagramId(string? className) => string.Format("class_only_{0}", className);
    public string ClassOnlyDiagramTitle(string? className) => $"Class diagram -> {className}";

    public string CompositeDomainPageLink(string? domain) => string.Format("/pages/composite_domain_{0}.html", domain);
    public string CompositeActionPageLink(string? action) => string.Format("/pages/composite_action_{0}.html", action);

    public string MindmapClassOnlyPumlFilename(string? className) => string.Format("mindmap_class_only_{0}.puml", className);

    public string MindmapActionPumlFilename(string? action) => string.Format("mindmap_action_{0}.puml", action?.AlphanumericOnly());
    public string MindmapActionDiagramId(string? action) => string.Format("mindmap_action_{0}", action?.AlphanumericOnly());

    public string MindmapDomainPumlFilename(string? domain) => string.Format("mindmap_domain_{0}.puml", domain?.AlphanumericOnly());
    public string MindmapDomainDiagramId(string? domain) => string.Format("mindmap_domain_{0}", domain?.AlphanumericOnly());

    public string NamespaceOnlyDiagramId(string? nameSpace) => string.Format("namespace_only_{0}", nameSpace?.AlphanumericOnly());

    public string NamespaceOnlyPumlFilename(string? nameSpace) => string.Format("namespace_only_{0}.puml", nameSpace?.AlphanumericOnly());

    public string NamespaceOnlyHtmlFilename(string? nameSpace) => string.Format("namespace_only_{0}.html", nameSpace?.AlphanumericOnly());
}
