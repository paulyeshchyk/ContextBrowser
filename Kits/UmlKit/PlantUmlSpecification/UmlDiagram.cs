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
    protected readonly HashSet<IUmlElement> Meta = new();

    protected virtual string SUmlStartTag { get => "@startuml"; }

    protected virtual string SUmlEndTag { get => "@enduml"; }

    protected int _nextOrder = 0;
    protected readonly SortedList<int, IUmlElement> _elements = new();
    protected readonly Dictionary<string, string> _skinParams = new();
    protected readonly List<IUmlElement> _relations = new();

    //none; . ; :;
    protected string? _separator; //по-умолчанию null
    protected string? _title;
    protected readonly DiagramBuilderOptions _options;

    public string DiagramId { get; private set; }

    public UmlDiagram(DiagramBuilderOptions options, string diagramId)
    {
        _options = options;
        DiagramId = diagramId;
    }
    // context: model, uml
    public bool IsUmlTagEnabled { get; set; } = true;

    // context: create, uml
    public abstract P AddParticipant(string name, string? alias = null, UmlParticipantKeyword keyword = UmlParticipantKeyword.Participant);

    // context: create, uml
    public abstract UmlDiagram<P> AddParticipant(P participant, string alias);

    public void SetLayoutDirection(UmlLayoutDirection.Direction direction)
    {
        Meta.Add(new UmlLayoutDirection(direction));
    }

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

    // context: uml, create
    public void Add(IUmlElement element)
    {
        _elements.Add(_nextOrder++, element);
    }

    public void AddRelations(IEnumerable<IUmlElement> relations)
    {
        _relations.AddRange(relations);
    }

    // context: uml, update
    public void SetTitle(string title) => _title = title;

    // context: uml, update
    public void SetSkinParam(string name, string value)
    {
        _skinParams[name] = value;
    }

    private bool _allowMixing = false;

    public void SetAllowMixing(bool value = true)
    {
        _allowMixing = value;
    }

    public void SetSeparator(string separtor)
    {
        _separator = separtor;
    }

    // context: uml, share
    public virtual void WriteTo(TextWriter writer)
    {
        WriteStart(writer);
        WriteAllowMixing(writer);
        WriteSkinElements(writer);
        WriteSeparator(writer);
        WriteTitle(writer);
        WriteBody(writer);
        WriteEnd(writer);
    }

    // context: uml, update
    protected virtual void WriteStart(TextWriter writer)
    {
        writer.WriteLine();

        if (IsUmlTagEnabled)
            writer.Write($"{SUmlStartTag}");
        writer.Write($" {{id={DiagramId}}}");
        writer.WriteLine();
    }

    private void WriteAllowMixing(TextWriter writer)
    {
        if (_allowMixing)
        {
            writer.WriteLine("allowmixing");
        }
    }

    // context: uml, update

    protected virtual void WriteEnd(TextWriter writer)
    {
        if (IsUmlTagEnabled)
            writer.Write(SUmlEndTag);
        writer.WriteLine();
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
