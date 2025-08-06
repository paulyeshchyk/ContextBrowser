using UmlKit.Extensions;

namespace UmlKit.Model;

public class UmlTransition : IUmlElement
{
    public readonly UmlState From;
    public readonly UmlState To;
    public readonly string? Label;
    private readonly UmlArrow _arrow;

    public string Declaration => $"{From.Alias}";

    public UmlTransition(UmlState from, UmlState to, UmlArrow arrow, string? label = null)
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

public class UmlTransitionEnd : UmlTransition
{
    public UmlTransitionEnd(UmlState from, UmlArrow arrow, string? label = null) : base(from, UmlStateFactory.AsterixState, arrow, label)
    {
    }
}

public class UmlTransitionStart : UmlTransition
{
    public UmlTransitionStart(UmlArrow arrow, UmlState to, string? label = null) : base(UmlStateFactory.AsterixState, to, arrow, label)
    {
    }
}