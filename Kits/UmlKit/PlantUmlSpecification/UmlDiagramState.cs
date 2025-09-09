using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;

namespace UmlKit.PlantUmlSpecification;

// context: model, uml
// pattern: Template method
// pattern note: subclassing
public class UmlDiagramState : UmlDiagram<UmlState>
{
    private readonly HashSet<UmlState> _states = new();
    private readonly HashSet<UmlTransitionState> _transitions = new();

    public UmlDiagramState(DiagramBuilderOptions options, string diagramId) : base(options, diagramId)
    {
    }

    public override void AddCallbreakNote(string name)
    {
        Meta.Add(new UmlNote(name, UmlNotePosition.Left, $"{name} -> {name}:"));
    }

    public override void AddSelfCallContinuation(string name, string methodName)
    {
#warning not working
    }

    public override UmlState AddParticipant(string name, string? alias = null, string? url = null, UmlParticipantKeyword keyword = UmlParticipantKeyword.Participant)
    {
        var result = new UmlState(name, alias, url);
        _states.Add(result);
        return result;
    }

    public override UmlDiagram<UmlState> AddParticipant(UmlState participant, string alias)
    {
        if (participant is UmlState theParticipant)
        {
            _states.Add(theParticipant);
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
        _transitions.Add(result);
        return result;
    }

    public override IUmlElement? Activate(string source, string destination, string reason, bool softActivation)
    {
#warning not implemented
        return null;
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

    public override void WriteBody(TextWriter writer, UmlWriteOptions writeOptions)
    {
        if (!_states.Any() || !_transitions.Any() || !Meta.Any())
        {
            //рисовать нечего
            return;
        }

        // 0. Meta
        foreach (var meta in Meta.Distinct())
            meta.WriteTo(writer, writeOptions);

        writer.WriteLine();

        // 1. Объявляем состояния
        foreach (var state in _states.Distinct())
            writer.WriteLine(state.Declaration);

        writer.WriteLine();

        // 2. Собираем состояния начальные и конечные
        var entryTransitions = _transitions
            .Where(t => string.Equals(t.Label, "entry", StringComparison.OrdinalIgnoreCase))
            .ToList();

        var exitTransitions = _transitions
            .Where(t => string.Equals(t.Label, "exit", StringComparison.OrdinalIgnoreCase))
            .ToList();

        var possibleStarts = entryTransitions
            .Select(t => t.To.Alias)
            .Distinct()
            .ToList();

        var possibleEnds = exitTransitions
            .Select(t => t.From.Alias)
            .Distinct()
            .ToList();

        // 3. Старт: от [*] ко всем начальным
        foreach (var start in possibleStarts)
        {
            var startTransition = new UmlTransitionStateStart(new UmlArrow(), new UmlState(start, null));
            startTransition.WriteTo(writer, writeOptions);
        }

        // 4. Основные переходы
        foreach (var transition in _transitions)
        {
            transition.WriteTo(writer, writeOptions);
        }

        // 5. Финиш: от конечных к [*]
        foreach (var end in possibleEnds)
        {
            var startTransition = new UmlTransitionStateEnd(new UmlState(end, null), new UmlArrow());
            startTransition.WriteTo(writer, writeOptions);
        }
    }

    public override IUmlElement AddLine(string line)
    {
        var result = new UmlLine(line);
        return result;
    }

    public override IUmlElement AddTransition(IUmlTransition<UmlState> transition)
    {
        _transitions.Add((UmlTransitionState)transition);
        Add(transition);
        return transition;
    }
}