using ContextKit.Model;

namespace UmlKit.Builders.Model;

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
}
