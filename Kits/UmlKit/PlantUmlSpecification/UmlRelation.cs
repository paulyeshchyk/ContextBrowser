using System.IO;

namespace UmlKit.Model;

// context: model, uml
// pattern: Composite leaf
public class UmlRelation : IUmlElement
{
    private const string SFakeRelationName = "FakeRelationName";

    public string From { get; }

    public string To { get; }

    public UmlArrow Arrow { get; }

    public UmlRelation(string? from, string to, UmlArrow arrow)
    {
        From = from ?? SFakeRelationName;
        To = to;
        Arrow = arrow;
    }

    // context: uml, share
    public void WriteTo(TextWriter writer, int alignNameMaxWidth)
    {
        writer.WriteLine();
        writer.Write($"{From}");
        Arrow.WriteTo(writer,alignNameMaxWidth);
        writer.Write($"{To}");
    }
}