using UmlKit.Model;
using UmlKit.Model.Options;

namespace UmlKit.Diagrams;

// context: model, uml
// pattern: Template method
// pattern note: subclassing
public class UmlDiagramSequence : UmlDiagram
{
    private readonly Dictionary<string, UmlParticipant> _participants = new();
    private readonly List<UmlSequence> _transitions = new();
    private readonly List<UmlActivate> _activations = new();
    private readonly List<UmlDeactivate> _deactivations = new();

    public UmlDiagramSequence(ContextTransitionDiagramBuilderOptions options)
        : base(options)
    {
    }

    public override void AddSelfCallBreak(string name)
    {
        Add(new UmlNote(name, UmlNotePosition.Left, $"{name} -> {name}:"));
    }

    public override void AddSelfCallContinuation(string name)
    {
        var from = new UmlParticipant(name);
        var to = from;
        var selfTransition = new UmlSequence(from, to, new UmlArrow());
        Add(selfTransition);

        if(_options.UseActivation)
            Activate(name);
    }

    public override IUmlElement AddParticipant(string? name, UmlParticipantKeyword keyword = UmlParticipantKeyword.Participant)
    {
        if(string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        if(_participants.ContainsKey(name))
            return _participants[name];

        var result = new UmlParticipant(name, keyword);
        _participants[name] = result;
        Add(result);
        return result;
    }

    public override UmlDiagram AddParticipant(IUmlElement participant)
    {
        if(participant is UmlParticipant p && !_participants.ContainsKey(p.ShortName))
        {
            _participants[p.ShortName] = p;
            Add(p);
        }
        return this;
    }

    public override IUmlElement AddTransition(string? from, string? to, string? label = null)
    {
        if(string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
            throw new ArgumentException($"From({from ?? string.Empty}) and To({to ?? string.Empty}) must not be null or empty");

        var f = AddParticipant(from);
        var t = AddParticipant(to);
        return AddTransition((IUmlDeclarable)f, (IUmlDeclarable)t, label);
    }

    public override IUmlElement AddTransition(IUmlDeclarable from, IUmlDeclarable to, string? label = null)
    {
        var transition = new UmlSequence(from, to, new UmlArrow(), label);
        _transitions.Add(transition);
        Add(transition);
        return transition;
    }

    public override IUmlElement? Activate(string from) =>
        Activate(new UmlDeclarableDto("activation", from));

    public override IUmlElement? Deactivate(string from) =>
        Deactivate(new UmlDeclarableDto("deactivation", from));

    public override IUmlElement? Activate(IUmlDeclarable from)
    {
        if(!_options.UseActivation)
            return null;

        var act = new UmlActivate(from.ShortName);
        _activations.Add(act);
        Add(act);
        return act;
    }

    public override IUmlElement? Deactivate(IUmlDeclarable from)
    {
        if(!_options.UseActivation)
            return null;

        var deact = new UmlDeactivate(from.ShortName);
        _deactivations.Add(deact);
        Add(deact);
        return deact;
    }

    public override void WriteBody(TextWriter writer)
    {
        writer.WriteLine("autonumber");

        // Уже отсортировано при добавлении
        foreach(var element in _elements.OrderBy(e => e.Key).Select(e => e.Value))
        {
            element.WriteTo(writer);
        }
    }
}
