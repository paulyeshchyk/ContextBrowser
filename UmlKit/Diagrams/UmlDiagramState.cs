using UmlKit.Model;
using UmlKit.Model.Options;

namespace UmlKit.Diagrams;

// context: model, uml
// pattern: Template method
// pattern note: subclassing
public class UmlDiagramState : UmlDiagram
{
    private readonly HashSet<UmlState> _states = new();
    private readonly HashSet<UmlTransition> _transitions = new();

    public UmlDiagramState(ContextTransitionDiagramBuilderOptions options) : base(options)
    {
    }

    public override void AddSelfCallBreak(string name)
    {
        Add(new UmlNote(name, UmlNotePosition.Left, $"{name} -> {name}:"));
    }

    public override void AddSelfCallContinuation(string name)
    {
#warning not working
        //throw new NotImplementedException();
    }

    public override IUmlElement AddParticipant(string? name, UmlParticipantKeyword keyword = UmlParticipantKeyword.Participant)
    {
        var result = new UmlState(name);
        _states.Add(result);
        return result;
    }

    public override UmlDiagram AddParticipant(IUmlElement participant)
    {
        if(participant is UmlState theParticipant)
        {
            _states.Add(theParticipant);
            return this;
        }

        throw new ArgumentException($"UmlState is supported only {nameof(participant)}");
    }

    public override IUmlElement AddTransition(string? from, string? to, string? label = null)
    {
        var theFrom = new UmlState(from);
        var theTo = new UmlState(to);
        return AddTransition(theFrom, theTo, label);
    }

    public override IUmlElement AddTransition(IUmlDeclarable from, IUmlDeclarable to, string? label = null)
    {
        if(from is not UmlState theFrom)
        {
            throw new ArgumentException($"неподдерживаемый тип {nameof(from)}");
        }

        if(to is not UmlState theTo)
        {
            throw new ArgumentException($"неподдерживаемый тип {nameof(from)}");
        }

        var result = new UmlTransition(theFrom, theTo, new UmlArrow(), label);
        _transitions.Add(result);
        return result;
    }

    public override IUmlElement Activate(string from)
    {
        var dto = new UmlDeclarableDto("empty declaration", from);
        return Activate(dto);
    }

    public override IUmlElement Deactivate(string from)
    {
        var dto = new UmlDeclarableDto("empty declaration", from);
        return Deactivate(dto);
    }

    public override IUmlElement Activate(IUmlDeclarable from)
    {
        return new UmlActivate(from.ShortName);
    }

    public override IUmlElement Deactivate(IUmlDeclarable from)
    {
        return new UmlDeactivate(from.ShortName);
    }

    public override void WriteBody(TextWriter writer)
    {
        if(!_states.Any() || !_transitions.Any())
        {
            //рисовать нечего
            return;
        }

        // 1. Объявляем состояния
        foreach(var state in _states.Distinct())
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
            .Select(t => t.To.ShortName)
            .Distinct()
            .ToList();

        var possibleEnds = exitTransitions
            .Select(t => t.From.ShortName)
            .Distinct()
            .ToList();

        // 3. Старт: от [*] ко всем начальным
        foreach(var start in possibleStarts)
        {
            var startTransition = new UmlTransitionStart(new UmlArrow(), new UmlState(start));
            startTransition.WriteTo(writer);
        }

        // 4. Основные переходы
        foreach(var transition in _transitions)
        {
            transition.WriteTo(writer);
        }

        // 5. Финиш: от конечных к [*]
        foreach(var end in possibleEnds)
        {
            var startTransition = new UmlTransitionEnd(new UmlState(end), new UmlArrow());
            startTransition.WriteTo(writer);
        }
    }
}