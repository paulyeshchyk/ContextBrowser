using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;

namespace UmlKit.PlantUmlSpecification;

// context: uml, model
public class UmlDiagramClass : UmlDiagram<UmlState>
{
    private readonly HashSet<UmlState> _states = new();
    private readonly List<UmlTransitionState> _transitions = new();

    public UmlDiagramClass(DiagramBuilderOptions options, string diagramId) : base(options, diagramId)
    {
    }

    public override void AddCallbreakNote(string name)
    {
#warning that is not correct for Class diagram
        Add(new UmlNote(name, UmlNotePosition.Left, $"{name} -> {name}:"));
    }

    public override void AddSelfCallContinuation(string name, string methodName)
    {
#warning not working
    }

    // context: uml, create
    public override UmlState AddParticipant(string name, string? alias = null, string? url = null, UmlParticipantKeyword keyword = UmlParticipantKeyword.Participant)
    {
        var result = new UmlState(name, alias, url);
        _states.Add(result);
        return result;
    }

    public override UmlDiagram<UmlState> AddParticipant(UmlState participant, string alias)
    {
        if (participant is UmlState state)
        {
            _states.Add(state);
            return this;
        }

        throw new ArgumentException($"UmlState is supported only {nameof(participant)}");
    }

    public override IUmlElement AddTransition(UmlState from, UmlState to, bool isAsync = false, string? label = null)
    {
        if (from is not UmlState theFrom)
        {
            throw new ArgumentException($"неподдерживаемый тип {nameof(from)}");
        }

        if (to is not UmlState theTo)
        {
            throw new ArgumentException($"неподдерживаемый тип {nameof(from)}");
        }

        var result = new UmlTransitionState(theFrom, theTo, new UmlArrow(), label);
        return AddTransition(result);
    }

    public override IUmlElement AddTransition(IUmlTransition<UmlState> transition)
    {
        _transitions.Add((UmlTransitionState)transition);
        Add(transition);
        return transition;
    }

    public override IUmlElement? Activate(string source, string destination, string reason, bool softActivation)
    {
        throw new NotImplementedException();
    }

    public override IUmlElement Activate(string from, string reason, bool softActivation)
    {
        var dto = new UmlDeclarableDto("empty declaration", from);
        return Activate(dto, reason, softActivation);
    }

    public override IUmlElement Deactivate(string from)
    {
        var dto = new UmlDeclarableDto("empty declaration", from);
        return Deactivate(dto);
    }

    public override IUmlElement Activate(IUmlDeclarable from, string reason, bool softActivation)
    {
        return new UmlActivate(null, from.Alias, reason, softActivation);
    }

    public override IUmlElement Deactivate(IUmlDeclarable from)
    {
        return new UmlDeactivate(from.Alias);
    }

    // context: uml, share
    public override void WriteBody(TextWriter writer, UmlWriteOptions writeOptions)
    {
        foreach (var meta in Meta.Distinct())
            meta.WriteTo(writer, writeOptions);

        writer.WriteLine();

        // объекты
        writer.WriteLine();
        foreach (var element in Elements.Values)
        {
            element.WriteTo(writer, writeOptions);
        }

        // связи
        writer.WriteLine();
        foreach (var relation in _relations)
        {
            relation.WriteTo(writer, writeOptions);
        }
    }

    public override IUmlElement AddLine(string line)
    {
        var result = new UmlLine(line);
        return result;
    }
}