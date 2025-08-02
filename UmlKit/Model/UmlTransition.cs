using ContextBrowser.Infrastructure.Extensions;

namespace UmlKit.Model;

public class UmlTransition : IUmlElement
{
    public readonly UmlState From;
    public readonly UmlState To;
    public readonly string? Label;
    private readonly UmlArrow _arrow;

    public string Declaration => $"{From.ShortName}";

    public UmlTransition(UmlState from, UmlState to, UmlArrow arrow, string? label = null)
    {
        From = from;
        To = to;
        Label = label;
        _arrow = arrow;
    }

    public void WriteTo(TextWriter writer)
    {
        var theLabel = this.Label is not null ? $" : {this.Label.AlphanumericOnly()}" : string.Empty;
        writer.Write($"{From.ShortName} ");
        _arrow.WriteTo(writer);
        writer.Write($" {To.ShortName}");
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