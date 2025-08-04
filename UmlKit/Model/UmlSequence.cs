namespace UmlKit.Model;

public class UmlSequence : IUmlElement
{
    public readonly IUmlDeclarable From;
    public readonly IUmlDeclarable To;
    public readonly string? Label;
    public readonly UmlArrow Arrow;

    public UmlSequence(IUmlDeclarable from, IUmlDeclarable to, UmlArrow arrow, string? label = null)
    {
        From = from;
        To = to;
        Label = label;
        Arrow = arrow;
    }

    public void WriteTo(TextWriter writer)
    {
        var arrowL = Label is not null ? $" : {Label}" : string.Empty;
        writer.Write($"{From.ShortName}");
        Arrow.WriteTo(writer);
        writer.Write($"{To.ShortName}{arrowL}");
        writer.WriteLine();
    }
}
