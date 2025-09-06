using ContextBrowserKit.Extensions;
using ContextKit.Model;
using ExporterKit.Uml.Model;
using UmlKit;
using UmlKit.Exporter;

namespace ExporterKit.Uml.Model;

public static class UmlUrlBuilder
{
    public static string BuildUrl(ContextInfo contextInfo)
    {
        return $"class_only_{contextInfo.FullName.AlphanumericOnly()}.html";
    }

    public static string? BuildClassUrl(UmlClassDiagramElementDto? dto)
    {
        return dto == null
            ? null
            : $"class_only_{dto.FullName.AlphanumericOnly()}.html";
    }

    public static string? BuildNamespaceUrl(string? nameSpace)
    {
        return string.IsNullOrWhiteSpace(nameSpace)
            ? null
            : $"namespace_only_{nameSpace.AlphanumericOnly()}.html";
    }
}