using System.IO;
using ContextBrowserKit.Extensions;

namespace UmlKit.PlantUmlSpecification;

public class UmlTransitionState<P> : IUmlTransition<P>
    where P : IUmlParticipant
{
    public P From { get; }

    public P To { get; }

    public readonly string? Label;

    private readonly UmlArrow _arrow;

    public UmlTransitionState(P from, P to, UmlArrow arrow, string? label = null)
    {
        From = from;
        To = to;
        Label = label;
        _arrow = arrow;
    }

    public void WriteTo(TextWriter writer, UmlWriteOptions writeOptions)
    {
        var theLabel = Label is not null ? $" : {Label.AlphanumericOnly()}" : string.Empty;
        writer.Write($"{From.Alias} ");
        _arrow.WriteTo(writer, writeOptions);
        writer.Write($" {To.Alias}");
        writer.Write(theLabel);
        writer.WriteLine();
    }
}

public class UmlTransitionStateEnd : UmlTransitionState<UmlState>
{
    public UmlTransitionStateEnd(UmlState from, UmlArrow arrow, string? label = null) : base(from, UmlStateFactory.AsterixState, arrow, label)
    {
    }
}

public class UmlTransitionStateStart : UmlTransitionState<UmlState>
{
    public UmlTransitionStateStart(UmlArrow arrow, UmlState to, string? label = null) : base(UmlStateFactory.AsterixState, to, arrow, label)
    {
    }
}