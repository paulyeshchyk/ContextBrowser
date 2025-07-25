using ContextBrowser.UmlKit.Model;

namespace ContextBrowser.UmlKit.Diagrams;

// context: model, uml
// pattern: Template method
// pattern note: subclassing
public class UmlDiagramState : UmlDiagram
{
    private readonly HashSet<UmlState> _states = new();
    private readonly HashSet<UmlTransition> _transitions = new();

    public override UmlDiagram AddParticipant(string name)
    {
        _states.Add(new UmlState(name));
        return this;
    }

    public override UmlDiagram AddTransition(string from, string to, string? label = null)
    {
        _transitions.Add(new UmlTransition(new UmlState(from), new UmlState(to), new UmlArrow(), label));
        return this;
    }


    public override void WriteBody(TextWriter writer)
    {
        if(!_states.Any() || !_transitions.Any())
        {
            Console.WriteLine($"Состояния и переходы должны быть определены для {this._title}");
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
