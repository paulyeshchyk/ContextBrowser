namespace ContextBrowser.UmlKit.Model;

// context: model, uml
// pattern: Composite leaf
public class UmlRelation : IUmlElement
{
    public string From { get; }

    public string To { get; }

    public UmlRelation(string? from, string to)
    {
        From = from ?? "???";
        To = to;
    }

    // context: uml, share
    public void WriteTo(TextWriter writer)
    {
        writer.WriteLine($"{From} --> {To}");
    }
}