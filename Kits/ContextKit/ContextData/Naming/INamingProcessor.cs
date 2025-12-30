using System;

namespace ContextKit.ContextData.Naming;

public interface INamingProcessor
{
    string ClassActionPumlFilename(string action);
    string ClassActionDiagramId(string action);

    string ClassDomainPumlFilename(string domain);
    string ClassDomainDiagramId(string domain);

    string ClassActionDomainPumlFilename(string action, string domain);
    string ClassActionDomainDiagramId(string action, string domain);
    string ClassActionDomainDiagramTitle(string action, string domain);

    string ClassOnlyHtmlFilename(string? className);
    string ClassOnlyPumlFilename(string? className);
    string ClassOnlyDiagramId(string? className);
    string ClassOnlyDiagramTitle(string? className);

    string CompositeDomainPageLink(string? domain);
    string CompositeActionPageLink(string? action);

    string NamespaceOnlyDiagramId(string? nameSpace);
    string NamespaceOnlyPumlFilename(string? nameSpace);

    string MindmapClassOnlyPumlFilename(string? className);

    string MindmapActionPumlFilename(string? action);
    string MindmapActionDiagramId(string? action);

    string MindmapDomainPumlFilename(string? domain);
    string MindmapDomainDiagramId(string? domain);

}
