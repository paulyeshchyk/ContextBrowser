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

        foreach(var state in _states.Distinct())
            writer.WriteLine(state.Declaration);

        writer.WriteLine();

        if(_transitions.FirstOrDefault() is UmlTransition start)
        {
            writer.WriteLine($"[*] -> {start.From.ShortName}");
        }

        foreach(var transition in _transitions)
        {
            var arrow = transition.Label is not null ? $" : {transition.Label.StateShortName()}" : string.Empty;
            writer.WriteLine($"{transition.From.ShortName} -> {transition.To.ShortName}{arrow}");
        }

        if(_transitions.LastOrDefault() is UmlTransition end)
        {
            writer.WriteLine($"{end.To.ShortName} -> [*]");
        }
    }
}
