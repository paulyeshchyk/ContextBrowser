using ContextBrowser.UmlKit.Model;

namespace ContextBrowser.UmlKit.Diagrams;

// context: uml, model
public class UmlDiagramClasses : UmlDiagram
{
    private readonly HashSet<UmlState> _states = new();
    private readonly List<(string From, string To, string? Label)> _transitions = new();

    // context: uml, create
    public override UmlDiagram AddParticipant(string name)
    {
        _states.Add(new UmlState(name));
        return this;
    }

    // context: uml, create
    public override UmlDiagram AddTransition(string from, string to, string? label = null)
    {
        _transitions.Add((from, to, label));
        return this;
    }

    // context: uml, share
    public override void WriteBody(TextWriter writer)
    {
        writer.WriteLine();

        foreach(var element in _elements)
        {
            element.WriteTo(writer);
        }
    }
}