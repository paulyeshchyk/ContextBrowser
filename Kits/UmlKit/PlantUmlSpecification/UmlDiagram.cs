using System.Collections.Generic;
using System.IO;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;

namespace UmlKit.PlantUmlSpecification;

// context: model, uml
// pattern: Template method
// parsing: error
public abstract class UmlDiagram<P>
    where P : IUmlParticipant
{
    // context: create, uml
    public abstract P AddParticipant(string name, string? alias = null, UmlParticipantKeyword keyword = UmlParticipantKeyword.Participant);

    // context: create, uml
    public abstract UmlDiagram<P> AddParticipant(P participant, string alias);

    public abstract void AddCallbreakNote(string name);

    public abstract void AddSelfCallContinuation(string name, string methodName);

    // context: create, uml
    public abstract IUmlElement AddTransition(IUmlTransition<P> transition);

    // context: create, uml
    public abstract IUmlElement AddTransition(P from, P to, bool isAsync = false, string? label = null);

    // context: create, uml
    //public abstract IUmlElement AddTransition(string? from, string? to, bool isAsync = false, string? label = null);

    public virtual IUmlElement AddComment(string line)
    {
        var result = new UmlComment(line);
        Add(result);
        return result;
    }

    public abstract IUmlElement AddLine(string line);

    // context: share, uml
    public abstract IUmlElement? Activate(string destination, string reason, bool softActivation);

    // context: share, uml
    public abstract IUmlElement? Activate(string source, string destination, string reason, bool softActivation);

    // context: share, uml
    public abstract IUmlElement? Deactivate(string from);

    // context: share, uml
    public abstract IUmlElement? Activate(IUmlDeclarable from, string reason, bool softActivation);

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
        foreach (var item in result)
        {
            Add(item);
        }
        return result;
    }

    public string DiagramId { get; private set; }

    public UmlDiagram(DiagramBuilderOptions options, string diagramId)
    {
        _options = options;
        DiagramId = diagramId;
    }

    protected virtual string SUmlStartTag { get => "@startuml"; }

    protected virtual string SUmlEndTag { get => "@enduml"; }

    protected int _nextOrder = 0;

    protected readonly SortedList<int, IUmlElement> _elements = new();

    protected readonly Dictionary<string, string> _skinParams = new();

    //none; . ; :;
    protected string? _separator; //по-умолчанию null
    protected string? _title;
    protected readonly DiagramBuilderOptions _options;

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

    public void SetSeparator(string separtor)
    {
        _separator = separtor;
    }

    // context: uml, share
    public virtual void WriteTo(TextWriter writer)
    {
        WriteStart(writer);
        WriteSkinElements(writer);
        WriteSeparator(writer);
        WriteTitle(writer);
        WriteBody(writer);
        WriteEnd(writer);
    }

    // context: uml, update
    protected virtual void WriteStart(TextWriter writer)
    {
        writer.Write(SUmlStartTag);
        writer.WriteLine($" {{id={DiagramId}}}");
    }

    // context: uml, update
    protected virtual void WriteEnd(TextWriter writer)
    {
        writer.WriteLine(SUmlEndTag);
    }

    protected virtual void WriteSeparator(TextWriter writer)
    {
        if (_separator != null)
        {
            writer.WriteLine($"set separator {_separator}");
        }
    }
    // context: uml, update
    protected virtual void WriteSkinElements(TextWriter writer)
    {
        foreach (var kvp in _skinParams)
            writer.WriteLine($"skinparam {kvp.Key} {kvp.Value}");
    }

    // context: uml, update
    protected virtual void WriteTitle(TextWriter writer)
    {
        if (!string.IsNullOrWhiteSpace(_title))
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
