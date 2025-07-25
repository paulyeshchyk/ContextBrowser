using ContextBrowser.UmlKit.Extensions;
using ContextBrowser.UmlKit.Model;

namespace ContextBrowser.UmlKit.Diagrams;

// context: model, uml
// pattern: Template method
// pattern note: subclassing
public class UmlDiagramState : UmlDiagram
{
    private readonly List<UmlState> _states = new();
    private readonly List<(UmlState From, UmlState To, string? Label)> _transitions = new();

    public override UmlDiagram AddParticipant(string name)
    {
        _states.Add(new UmlState(name));
        return this;
    }

    public override UmlDiagram AddTransition(string from, string to, string? label = null)
    {
        _transitions.Add((new UmlState(from), new UmlState(to), label));
        return this;
    }


    public override void WriteBody(TextWriter writer)
    {
        if (!_states.Any() || !_transitions.Any())
        {
            Console.WriteLine($"Состояния и переходы должны быть определены для {this._title}");
            return;
        }

        foreach (var state in _states.Distinct())
            writer.WriteLine($"state {state.FullName}");

        writer.WriteLine();

        writer.WriteLine($"[*] -> {_transitions.FirstOrDefault().From.ShortName}");
        foreach (var (from, to, label) in _transitions)
        {
            var arrow = label is not null ? $" : {label.StateShortName()}" : string.Empty;
            writer.WriteLine($"{from.ShortName} -> {to.ShortName}{arrow}");
        }

        writer.WriteLine($"{_transitions.LastOrDefault().To.ShortName} -> [*]");
    }
}
