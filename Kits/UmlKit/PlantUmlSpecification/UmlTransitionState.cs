using System.IO;
using ContextBrowserKit.Extensions;

namespace UmlKit.Model;

public class UmlTransitionState : IUmlTransition<UmlState>
{
    public UmlState From { get; }

    public UmlState To { get; }

    public readonly string? Label;

    private readonly UmlArrow _arrow;

    public UmlTransitionState(UmlState from, UmlState to, UmlArrow arrow, string? label = null)
    {
        From = from;
        To = to;
        Label = label;
        _arrow = arrow;
    }

    public void WriteTo(TextWriter writer)
    {
        var theLabel = Label is not null ? $" : {Label.AlphanumericOnly()}" : string.Empty;
        writer.Write($"{From.Alias} ");
        _arrow.WriteTo(writer);
        writer.Write($" {To.Alias}");
        writer.Write(theLabel);
        writer.WriteLine();
    }
}

public class UmlTransitionStateEnd : UmlTransitionState
{
    public UmlTransitionStateEnd(UmlState from, UmlArrow arrow, string? label = null) : base(from, UmlStateFactory.AsterixState, arrow, label)
    {
    }
}

public class UmlTransitionStateStart : UmlTransitionState
{
    public UmlTransitionStateStart(UmlArrow arrow, UmlState to, string? label = null) : base(UmlStateFactory.AsterixState, to, arrow, label)
    {
    }
}