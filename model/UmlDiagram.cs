namespace ContextBrowser.model;

// context: model, uml
public class UmlDiagram
{
    private const string SUmlStartTag = "@startuml";
    private const string SUmlEndTag = "@enduml";
    private readonly List<IUmlElement> _elements = new();
    private readonly Dictionary<string, string> _skinParams = new();
    private string? _title;

    // context: uml, create
    public void Add(IUmlElement element) => _elements.Add(element);

    // context: uml, update
    public void SetTitle(string title) => _title = title;

    // context: uml, update
    public void SetSkinParam(string name, string value) => _skinParams[name] = value;

    // context: uml, share
    public void WriteTo(TextWriter writer)
    {
        writer.WriteLine(SUmlStartTag);

        foreach(var kvp in _skinParams)
            writer.WriteLine($"skinparam {kvp.Key} {kvp.Value}");

        if(!string.IsNullOrWhiteSpace(_title))
            writer.WriteLine($"title {_title}");

        foreach(var element in _elements)
            element.WriteTo(writer);

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

public interface IUmlElement
{
    void WriteTo(TextWriter writer);
}

// context: model, uml
public class UmlComponent : IUmlElement
{
    public string Name { get; }

    public string? Url { get; }

    public UmlComponent(string? name, string? url = null)
    {
        Name = name ?? "???";
        Url = url;
    }

    // context: uml, share
    public void WriteTo(TextWriter writer)
    {
        if(!string.IsNullOrWhiteSpace(Url))
            writer.WriteLine($"component \"{Name}\" [[{Url}]]");
        else
            writer.WriteLine($"component \"{Name}\"");
    }
}

// context: model, uml
public class UmlRelation : IUmlElement
{
    public string From { get; }

    public string To { get; }

    public UmlRelation(string? from, string to)
    {
        From = from ?? "???";
        To = to;
    }

    // context: uml, share
    public void WriteTo(TextWriter writer)
    {
        writer.WriteLine($"{From} --> {To}");
    }
}

// context: model, uml
public class UmlPackage : IUmlElement
{
    public string Name { get; }

    public List<IUmlElement> Elements { get; } = new();

    public UmlPackage(string name) => Name = name;

    // context: uml, create
    public void Add(IUmlElement e) => Elements.Add(e);

    // context: uml, share
    public void WriteTo(TextWriter writer)
    {
        writer.WriteLine($"package \"{Name}\" {{");
        foreach(var element in Elements)
            element.WriteTo(writer);
        writer.WriteLine("}");
    }
}

// context: model, uml
public class UmlComponentGroup : IUmlElement
{
    public string Name { get; }

    public string Stereotype { get; }

    public List<IUmlElement> Members { get; } = new();

    public UmlComponentGroup(string? name, string stereotype)
    {
        Name = name ?? "???";
        Stereotype = stereotype;
    }

    // context: uml, create
    public void Add(IUmlElement e) => Members.Add(e);

    // context: uml, share
    public void WriteTo(TextWriter writer)
    {
        writer.WriteLine($"  component \"{Name}\" <<{Stereotype}>> {{");
        foreach(var member in Members)
            member.WriteTo(writer);
        writer.WriteLine("  }");
    }
}

// context: model, uml
public class UmlMethodBox : IUmlElement
{
    public string Name { get; }

    public string? Stereotype { get; }

    public UmlMethodBox(string? name, string? stereotype = null)
    {
        Name = name ?? "???";
        Stereotype = stereotype;
    }

    // context: uml, share
    public void WriteTo(TextWriter writer)
    {
        if(!string.IsNullOrWhiteSpace(Stereotype))
            writer.WriteLine($"    [{Name}] <<{Stereotype}>>");
        else
            writer.WriteLine($"    [{Name}]");
    }
}