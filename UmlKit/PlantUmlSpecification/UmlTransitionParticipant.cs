namespace UmlKit.Model;

public class UmlTransitionParticipant : IUmlTransition<UmlParticipant>
{
    public UmlParticipant From { get; }

    public UmlParticipant To { get; }

    public readonly string? Label;

    public readonly UmlArrow Arrow;

    public UmlTransitionParticipant(UmlParticipant from, UmlParticipant to, UmlArrow arrow, string? label = null)
    {
        From = from;
        To = to;
        Label = label;
        Arrow = arrow;
    }

    public void WriteTo(TextWriter writer)
    {
        var arrowL = Label is not null ? $" : {Label}" : string.Empty;
        writer.Write($"{From.Alias}");
        Arrow.WriteTo(writer);
        writer.Write($"{To.Alias}{arrowL}");
        writer.WriteLine();
    }
}
