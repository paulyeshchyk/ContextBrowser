namespace UmlKit.Model;

// context: model, uml
// pattern: Composite leaf
public class UmlRelation : IUmlElement
{
    private const string SFakeRelationName = "FakeRelationName";

    public string From { get; }

    public string To { get; }

    public UmlRelation(string? from, string to)
    {
        From = from ?? SFakeRelationName;
        To = to;
    }

    // context: uml, share
    public void WriteTo(TextWriter writer)
    {
        writer.WriteLine($"{From} --> {To}");
    }
}