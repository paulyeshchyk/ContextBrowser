using UmlKit.Model;
using UmlKit.Model.Options;

namespace UmlKit.Diagrams;

// context: model, uml
// pattern: Template method
public abstract class UmlDiagram
{
    // context: create, uml
    public abstract IUmlElement AddParticipant(string? name, UmlParticipantKeyword keyword = UmlParticipantKeyword.Participant);

    // context: create, uml
    public abstract UmlDiagram AddParticipant(IUmlElement participant);

    public abstract void AddSelfCallBreak(string name);

    public abstract void AddSelfCallContinuation(string name);

    // context: create, uml
    public abstract IUmlElement AddTransition(string? from, string? to, string? label = null);

    // context: create, uml
    public abstract IUmlElement AddTransition(IUmlDeclarable from, IUmlDeclarable to, string? label = null);

    // context: share, uml
    public abstract IUmlElement? Activate(string from);

    // context: share, uml
    public abstract IUmlElement? Deactivate(string from);

    // context: share, uml
    public abstract IUmlElement? Activate(IUmlDeclarable from);

    // context: share, uml
    public abstract IUmlElement? Deactivate(IUmlDeclarable from);

    public virtual IUmlElement? AddNoteOver(string caller, string note)
    {
        var result = new UmlNote(caller, UmlNotePosition.Over, note);
        Add(result);
        return result;
    }

    public virtual IEnumerable<IUmlElement>? AddNoteOver(string caller, string callee, string note)
    {
        var result = new List<IUmlElement>() { new UmlNote(caller, UmlNotePosition.Over, note), new UmlNote(callee, UmlNotePosition.Over, note), };
        foreach(var item in result)
        {
            Add(item);
        }
        return result;
    }

    public UmlDiagram(ContextTransitionDiagramBuilderOptions options)
    {
        _options = options;
    }

    protected virtual string SUmlStartTag { get => "@startuml"; }

    protected virtual string SUmlEndTag { get => "@enduml"; }

    protected int _nextOrder = 0;

    protected readonly SortedList<int, IUmlElement> _elements = new();

    protected readonly Dictionary<string, string> _skinParams = new();
    protected string? _title;
    protected readonly ContextTransitionDiagramBuilderOptions _options;
    // context: uml, create
    public void Add(IUmlElement element)
    {
        _elements.Add(_nextOrder++, element);
    }

    // context: uml, update
    public void SetTitle(string title) => _title = title;

    // context: uml, update
    public void SetSkinParam(string name, string value)
    {
        _skinParams[name] = value;
    }

    // context: uml, share
    public virtual void WriteTo(TextWriter writer)
    {
        WriteStart(writer);
        WriteSkinElements(writer);
        WriteTitle(writer);
        WriteBody(writer);
        WriteEnd(writer);
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
    public abstract void WriteBody(TextWriter writer);

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
