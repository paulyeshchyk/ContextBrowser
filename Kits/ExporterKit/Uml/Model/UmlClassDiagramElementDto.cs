using ContextKit.Model;
using ExporterKit;
using ExporterKit.Uml;
using ExporterKit.Uml.Model;
using UmlKit;

namespace ExporterKit.Uml.Model;

public record UmlClassDiagramElementDto
{
    public string Namespace { get; set; }

    public string ClassName { get; init; }

    public IContextInfo ContextInfo { get; init; }

    public UmlClassDiagramElementDto(string @namespace, string className, IContextInfo contextInfo)
    {
        Namespace = @namespace;
        ClassName = className;
        ContextInfo = contextInfo;
    }

    public string FullName { get { return $"{Namespace}.{ClassName}"; } }
}
