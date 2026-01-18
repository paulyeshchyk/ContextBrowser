using ContextBrowserKit.Extensions;

namespace ContextKit.ContextData.Naming;

public class NamingProcessor : INamingProcessor
{
    public string ClassActionDomainDiagramId(string action, string domain) => string.Format("class_{0}_{1}", action?.AlphanumericOnly(), domain?.AlphanumericOnly());

    public string ClassActionDomainDiagramTitle(string action, string domain) => string.Format("{0} -> {1}", action, domain);

    public string ClassActionDomainPumlFilename(string action, string domain) => string.Format("class_{0}_{1}.puml", action?.AlphanumericOnly(), domain?.AlphanumericOnly());

    public string ClassActionDomainPageLink(string action, string domain) => string.Format("pages\\class_{0}_{1}.html", action?.AlphanumericOnly(), domain?.AlphanumericOnly());

    public string ClassActionDomainHtmlFilename(string action, string domain) => string.Format("class_{0}_{1}.html", action?.AlphanumericOnly(), domain?.AlphanumericOnly());

    public string ClassActionDiagramId(string action) => string.Format("class_action_{0}", action.AlphanumericOnly());

    public string ClassActionPumlFilename(string action) => string.Format("class_action_{0}.puml", action.AlphanumericOnly());

    public string ClassDomainDiagramId(string domain) => string.Format("class_domain_{0}", domain.AlphanumericOnly());

    public string ClassDomainPumlFilename(string domain) => string.Format("class_domain_{0}.puml", domain.AlphanumericOnly());

    public string ClassOnlyDiagramId(string? className) => string.Format("class_only_{0}", className?.AlphanumericOnly());

    public string ClassOnlyDiagramTitle(string? className) => string.Format("Class diagram -> {0}", className?.AlphanumericOnly());

    public string ClassOnlyHtmlFilename(string? className) => string.Format("class_only_{0}.html", className?.AlphanumericOnly());

    public string ClassOnlyPumlFilename(string? className) => string.Format("class_only_{0}.puml", className?.AlphanumericOnly());

    public string CompositeActionDomainHtmlFile(string? action, string? domain) => string.Format("composite_{0}_{1}.html", action?.AlphanumericOnly(), domain?.AlphanumericOnly());

    public string CompositeActionDomainPageLink(string? action, string? domain) => string.Format("pages\\composite_{0}_{1}.html", action?.AlphanumericOnly(), domain?.AlphanumericOnly());

    public string CompositeActionHtmlFile(string? action) => string.Format("composite_action_{0}.html", action?.AlphanumericOnly());

    public string CompositeActionPageLink(string? action) => string.Format("pages\\composite_action_{0}.html", action?.AlphanumericOnly());

    public string CompositeDomainHtmlFile(string? domain) => string.Format("composite_domain_{0}.html", domain?.AlphanumericOnly());

    public string CompositeDomainPageLink(string? domain) => string.Format("pages\\composite_domain_{0}.html", domain?.AlphanumericOnly());

    public string MindmapActionDiagramId(string? action) => string.Format("mindmap_action_{0}", action?.AlphanumericOnly());

    public string MindmapActionPumlFilename(string? action) => string.Format("mindmap_action_{0}.puml", action?.AlphanumericOnly());

    public string MindmapClassOnlyPumlFilename(string? className) => string.Format("mindmap_class_only_{0}.puml", className?.AlphanumericOnly());

    public string MindmapDomainDiagramId(string? domain) => string.Format("mindmap_domain_{0}", domain?.AlphanumericOnly());

    public string MindmapDomainPumlFilename(string? domain) => string.Format("mindmap_domain_{0}.puml", domain?.AlphanumericOnly());

    public string NamespaceOnlyDiagramId() => "namespace_only";

    public string NamespaceOnlyHtmlFilename() => "namespace_only.html";

    public string NamespaceOnlyPumlFilename() => "namespace_only.puml";

    public string NamespaceOnlyItemDiagramId(string? nameSpace) => string.Format("namespace_only_{0}", nameSpace?.AlphanumericOnly());

    public string NamespaceOnlyItemHtmlFilename(string? nameSpace) => string.Format("namespace_only_{0}.html", nameSpace?.AlphanumericOnly());

    public string NamespaceOnlyItemPumlFilename(string? nameSpace) => string.Format("namespace_only_{0}.puml", nameSpace?.AlphanumericOnly());

    public string SequenceActionDiagramId(string? action) => string.Format("sequence_action_{0}", action?.AlphanumericOnly());

    public string SequenceActionPumlFilename(string? action) => string.Format("sequence_action_{0}.puml", action?.AlphanumericOnly());

    public string SequenceDomainDiagramId(string? domain) => string.Format("sequence_domain_{0}", domain?.AlphanumericOnly());

    public string SequenceDomainPumlFilename(string? domain) => string.Format("sequence_domain_{0}.puml", domain?.AlphanumericOnly());

    public string StateActionDiagramId(string? action) => string.Format("state_action_{0}", action?.AlphanumericOnly());

    public string StateActionPumlFilename(string? action) => string.Format("state_action_{0}.puml", action?.AlphanumericOnly());

    public string StateDomainDiagramId(string? domain) => string.Format("state_domain_{0}", domain?.AlphanumericOnly());

    public string StateDomainPumlFilename(string? domain) => string.Format("state_domain_{0}.puml", domain?.AlphanumericOnly());
}
