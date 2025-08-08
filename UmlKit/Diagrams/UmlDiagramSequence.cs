using UmlKit.Extensions;
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

    public void AddAnonymousCallContinuation(string name)
    {
        var from = new UmlParticipant(name, name.AlphanumericOnly());
        var to = from;
        var selfTransition = new UmlSequence(from, to, new UmlArrow(flowType: _options.UseAsync ? UmlArrowFlowType.Async : UmlArrowFlowType.Sync));
        Add(selfTransition);
    }

    public override void AddSelfCallContinuation(string name, string methodName)
    {
        var from = new UmlParticipant(string.Empty, string.Empty);
        var to = new UmlParticipant(name, name.AlphanumericOnly());
        var selfTransition = new UmlSequence(from, to, new UmlArrow(flowType: _options.UseAsync ? UmlArrowFlowType.Async : UmlArrowFlowType.Sync), methodName);
        Add(selfTransition);
    }

    public override IUmlElement AddParticipant(string name, string alias, UmlParticipantKeyword keyword = UmlParticipantKeyword.Participant)
    {
        if(string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        if(_participants.ContainsKey(name))
            return _participants[name];

        var result = new UmlParticipant(name, alias, keyword);
        _participants[name] = result;
        Add(result);
        return result;
    }

    public override UmlDiagram AddParticipant(IUmlElement participant, string alias)
    {
        if(participant is UmlParticipant p && !_participants.ContainsKey(p.Alias))
        {
            _participants[p.Alias] = p;
            Add(p);
        }
        return this;
    }

    public override IUmlElement AddTransition(string? from, string? to, bool isAsync = false, string? label = null)
    {
        if(string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
            throw new ArgumentException($"From({from ?? string.Empty}) and To({to ?? string.Empty}) must not be null or empty");

        var f = AddParticipant(from, from.AlphanumericOnly());
        var t = AddParticipant(to, to.AlphanumericOnly());
        return AddTransition((IUmlDeclarable)f, (IUmlDeclarable)t, isAsync, label);
    }

    public override IUmlElement AddTransition(IUmlDeclarable from, IUmlDeclarable to, bool isAsync = false, string? label = null)
    {
        var transition = new UmlSequence(from, to, new UmlArrow(flowType: isAsync ? UmlArrowFlowType.Async : UmlArrowFlowType.Sync), label);
        _transitions.Add(transition);
        Add(transition);
        return transition;
    }

    public override IUmlElement? Activate(string from, bool isSystemCall) => Activate(new UmlDeclarableDto("activation", from), isSystemCall);

    public override IUmlElement? Deactivate(string from) => Deactivate(new UmlDeclarableDto("deactivation", from));

    public override IUmlElement? Activate(IUmlDeclarable from, bool isSystemCall)
    {
        if(!_options.UseActivation)
            return null;

        //AddAnonymousCallContinuation(from.Alias);

#warning dirty hack with AlphaNumeric()
        var act = new UmlActivate(from.Alias.AlphanumericOnly(), isSystemCall);
        _activations.Add(act);
        Add(act);
        return act;
    }

    public override IUmlElement? Deactivate(IUmlDeclarable from)
    {
        if(!_options.UseActivation)
            return null;
#warning dirty hack with AlphaNumeric()
        var deact = new UmlDeactivate(from.Alias.AlphanumericOnly());
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
