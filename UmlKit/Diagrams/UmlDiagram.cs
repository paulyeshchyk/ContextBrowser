using ContextBrowser.UmlKit.Model;

namespace ContextBrowser.UmlKit.Diagrams;

// context: model, uml
// pattern: Template method
public class UmlDiagram
{
    protected virtual string SUmlStartTag { get => "@startuml"; }

    protected virtual string SUmlEndTag { get => "@enduml"; }

    private readonly List<IUmlElement> _elements = new();
    protected readonly Dictionary<string, string> _skinParams = new();
    protected string? _title;

    // context: uml, create
    public void Add(IUmlElement element) => _elements.Add(element);

    // context: uml, update
    public void SetTitle(string title) => _title = title;

    // context: uml, update
    public void SetSkinParam(string name, string value) => _skinParams[name] = value;

    // context: uml, share
    public void WriteTo(TextWriter writer)
    {
        WriteStart(writer);
        WriteSkinElements(writer);
        WriteTitle(writer);
        WriteBody(writer);
        WriteEnd(writer);
    }

    // context: uml, update
    public virtual void WriteBody(TextWriter writer)
    {
        foreach(var element in _elements)
            element.WriteTo(writer);
    }

    // context: uml, update
    protected virtual void WriteSkinElements(TextWriter writer)
    {
        foreach(var kvp in _skinParams)
            writer.WriteLine($"skinparam {kvp.Key} {kvp.Value}");
    }

    // context: uml, update
    protected virtual void WriteTitle(TextWriter writer)
    {
        if(!string.IsNullOrWhiteSpace(_title))
            writer.WriteLine($"title {_title}");
    }

    // context: uml, update
    protected virtual void WriteStart(TextWriter writer)
    {
        writer.WriteLine(SUmlStartTag);
    }

    // context: uml, update
    protected virtual void WriteEnd(TextWriter writer)
    {
        writer.WriteLine(SUmlEndTag);
    }

    // context: uml, read
    public string ToUmlString()
    {
        using var sw = new StringWriter();
        WriteTo(sw);
        return sw.ToString();
    }

    // context: uml, share
    public void WriteToFile(string path)
    {
        File.WriteAllText(path, ToUmlString());
    }
}