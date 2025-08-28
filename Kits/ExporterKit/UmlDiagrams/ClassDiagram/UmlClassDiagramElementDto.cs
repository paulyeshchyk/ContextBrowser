using ContextKit.Model;

namespace UmlKit.UmlDiagrams.ClassDiagram;

internal record UmlClassDiagramElementDto
{
    public string Namespace { get; set; }
    public string ClassName { get; init; }
    public ContextInfo Method { get; init; }

    public UmlClassDiagramElementDto(string @namespace, string className, ContextInfo method)
    {
        Namespace = @namespace;
        ClassName = className;
        Method = method;
    }
}
