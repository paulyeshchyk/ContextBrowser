using ContextBrowser.UmlKit.Extensions;
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
        _transitions.Add(new UmlTransition(new UmlState(from), new UmlState(to), label));
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

        // 2. Находим старты и финиши
        var fromSet = _transitions.Select(t => t.From.ShortName).ToHashSet();
        var toSet = _transitions.Select(t => t.To.ShortName).ToHashSet();

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
            writer.WriteLine($"[*] -> {start}");

        // 4. Основные переходы
        foreach(var transition in _transitions)
        {
            var arrow = transition.Label is not null ? $" : {transition.Label.StateShortName()}" : string.Empty;
            writer.WriteLine($"{transition.From.ShortName} -> {transition.To.ShortName}{arrow}");
        }

        // 5. Финиш: от конечных к [*]
        foreach(var end in possibleEnds)
            writer.WriteLine($"{end} -> [*]");
    }
}
