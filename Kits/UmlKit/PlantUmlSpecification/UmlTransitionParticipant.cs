using System.IO;

namespace UmlKit.PlantUmlSpecification;

public class UmlTransitionParticipant<P> : IUmlTransition<P>
    where P : IUmlParticipant
{
    public P From { get; }

    public P To { get; }

    public readonly string? Label;

    public readonly UmlArrow Arrow;

    public UmlTransitionParticipant(P from, P to, UmlArrow arrow, string? label = null)
    {
        From = from;
        To = to;
        Label = label;
        Arrow = arrow;
    }

    public void WriteTo(TextWriter writer, UmlWriteOptions writeOptions)
    {
        var arrowL = Label is not null ? $" : {Label}" : string.Empty;
        writer.Write($"{From.Alias}");
        Arrow.WriteTo(writer, writeOptions);
        writer.Write($"{To.Alias}{arrowL}");
        writer.WriteLine();
    }
}
