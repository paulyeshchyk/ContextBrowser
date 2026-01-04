namespace ContextKit.ContextData.Naming;

public interface INamingProcessor
{
    string ClassActionDomainDiagramId(string action, string domain);
    string ClassActionDomainDiagramTitle(string action, string domain);
    string ClassActionDomainPumlFilename(string action, string domain);
    string ClassActionDomainPageLink(string action, string domain);
    string ClassActionDomainHtmlFilename(string action, string domain);

    string ClassActionDiagramId(string action);
    string ClassActionPumlFilename(string action);

    string ClassDomainDiagramId(string domain);
    string ClassDomainPumlFilename(string domain);

    string ClassOnlyDiagramId(string? className);
    string ClassOnlyDiagramTitle(string? className);
    string ClassOnlyHtmlFilename(string? className);
    string ClassOnlyPumlFilename(string? className);

    string CompositeActionDomainPageLink(string? action, string domain);

    string CompositeActionDomainHtmlFile(string? action, string? domain);

    string CompositeActionHtmlFile(string? action);
    string CompositeActionPageLink(string? action);

    string CompositeDomainHtmlFile(string? domain);
    string CompositeDomainPageLink(string? domain);

    string MindmapActionDiagramId(string? action);
    string MindmapActionPumlFilename(string? action);

    string MindmapClassOnlyPumlFilename(string? className);

    string MindmapDomainDiagramId(string? domain);
    string MindmapDomainPumlFilename(string? domain);

    string NamespaceOnlyDiagramId(string? nameSpace);
    string NamespaceOnlyHtmlFilename(string? nameSpace);
    string NamespaceOnlyPumlFilename(string? nameSpace);

    string SequenceActionDiagramId(string? action);
    string SequenceActionPumlFilename(string? action);

    string SequenceDomainDiagramId(string? domain);
    string SequenceDomainPumlFilename(string? domain);

    string StateActionDiagramId(string? action);
    string StateActionPumlFilename(string? action);

    string StateDomainDiagramId(string? domain);
    string StateDomainPumlFilename(string? domain);
}