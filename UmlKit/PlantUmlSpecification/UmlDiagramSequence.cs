﻿using UmlKit.Infrastructure.Options;
using UmlKit.Model;

namespace UmlKit.PlantUmlSpecification;

// context: model, uml
// pattern: Template method
// pattern note: subclassing
public class UmlDiagramSequence : UmlDiagram<UmlParticipant>
{
    private readonly Dictionary<string, UmlParticipant> _participants = new();
    private readonly List<UmlTransitionParticipant> _transitions = new();
    private readonly List<UmlActivate> _activations = new();
    private readonly List<UmlDeactivate> _deactivations = new();

    public UmlDiagramSequence(DiagramBuilderOptions options)
        : base(options)
    {
    }

    public override void AddCallbreakNote(string name)
    {
        Add(new UmlNote(name, UmlNotePosition.Left, $"{name} -> {name}:"));
    }

    public void AddAnonymousCallContinuation(string name)
    {
        var from = new UmlParticipant(name);
        var to = from;
        var selfTransition = new UmlTransitionParticipant(from, to, new UmlArrow(flowType: _options.Indication.UseAsync ? UmlArrowFlowType.Async : UmlArrowFlowType.Sync));
        Add(selfTransition);
    }

    public override void AddSelfCallContinuation(string name, string methodName)
    {
        var from = new UmlParticipant(string.Empty);
        var to = new UmlParticipant(name);
        var selfTransition = new UmlTransitionParticipant(from, to, new UmlArrow(flowType: _options.Indication.UseAsync ? UmlArrowFlowType.Async : UmlArrowFlowType.Sync), methodName);
        Add(selfTransition);
    }

    public override UmlParticipant AddParticipant(string name, string? alias = null, UmlParticipantKeyword keyword = UmlParticipantKeyword.Participant)
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

    public override UmlDiagram<UmlParticipant> AddParticipant(UmlParticipant participant, string alias)
    {
        if(participant is UmlParticipant p && !_participants.ContainsKey(p.Alias))
        {
            _participants[p.Alias] = p;
            Add(p);
        }
        return this;
    }

    public override IUmlElement AddTransition(IUmlTransition<UmlParticipant> transition)
    {
        _transitions.Add((UmlTransitionParticipant)transition);
        Add(transition);
        return transition;
    }

    //public override IUmlElement AddTransition(string? from, string? to, bool isAsync = false, string? label = null)
    //{
    //    if(string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
    //        throw new ArgumentException($"From({from ?? string.Empty}) and To({to ?? string.Empty}) must not be null or empty");

    //    var f = AddParticipant(from);
    //    var t = AddParticipant(to);
    //    return AddTransition(f, t, isAsync, label);
    //}

    public override IUmlElement AddTransition(UmlParticipant from, UmlParticipant to, bool isAsync = false, string? label = null)
    {
        var transition = new UmlTransitionParticipant(from, to, new UmlArrow(flowType: isAsync ? UmlArrowFlowType.Async : UmlArrowFlowType.Sync), label);
        _transitions.Add(transition);
        Add(transition);
        return transition;
    }

    public override IUmlElement? Activate(string source, string destination, string reason, bool softActivation)
    {
        if(!_options.UseActivation)
            return null;

        var sourceParticipant = new UmlParticipant(source);
        var destinationParticipant = new UmlParticipant(destination);

        var act = new UmlActivate(source, destination, reason: reason, softActivation);
        _activations.Add(act);
        Add(act);
        return act;
    }

    public override IUmlElement? Activate(string destination, string reason, bool softActivation)
    {
        var destinationParticipant = new UmlParticipant(destination);
        return Activate(destinationParticipant, reason, softActivation);
    }

    public override IUmlElement? Deactivate(string from)
    {
        return Deactivate(new UmlParticipant(from));
    }

    public override IUmlElement? Activate(IUmlDeclarable from, string reason, bool softActivation)
    {
        if(!_options.UseActivation)
            return null;

        //AddAnonymousCallContinuation(from.Alias);

        var act = new UmlActivate(null, from.Alias, reason, softActivation);
        _activations.Add(act);
        Add(act);
        return act;
    }

    public override IUmlElement? Deactivate(IUmlDeclarable from)
    {
        if(!_options.UseActivation)
            return null;
        var deact = new UmlDeactivate(from.Alias);
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

    public override IUmlElement AddLine(string line)
    {
        var result = new UmlLine(line);
        Add(result);
        return result;
    }
}
