namespace ContextBrowser.UmlKit.Diagrams;

// context: model, uml
// pattern: Template method
// pattern note: subclassing
public class UmlDiagramSequence : UmlDiagram
{
    private readonly List<string> _states = new();
    private readonly List<(string From, string To, string? Label)> _transitions = new();

    public UmlDiagramSequence AddState(string name)
    {
        _states.Add(name);
        return this;
    }

    public UmlDiagramSequence AddTransition(string from, string to, string? label = null)
    {
        _transitions.Add((from, to, label));
        return this;
    }

    public override void WriteBody(TextWriter writer)
    {
        foreach(var state in _states.Distinct())
            writer.WriteLine($"state \"{state}\"");

        writer.WriteLine();

        foreach (var (from, to, label) in _transitions)
        {
            var arrow = label is not null ? $" : {label}" : string.Empty;
            writer.WriteLine($"\"{from}\" --> \"{to}\"{arrow}");
        }
    }
}