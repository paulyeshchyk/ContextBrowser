
namespace ContextBrowser.UmlKit.Model;

public class UmlTransition : IUmlElement
{
    public readonly UmlState From;
    public readonly UmlState To;
    public readonly string? Label;

    public string Declaration => $"{From.ShortName}";

    public UmlTransition(UmlState from, UmlState to, string? label = null)
    {
        From = from;
        To = to;
        Label = label;
    }


    public void WriteTo(TextWriter writer)
    {
        var arrow = Label is not null ? $" : {Label}" : string.Empty;
        writer.WriteLine($"{From.ShortName} --> {To.ShortName}{arrow}");
    }
}
