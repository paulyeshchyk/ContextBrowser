
namespace ContextBrowser.UmlKit.Model;

public class UmlSequence : IUmlElement
{
    public readonly IUmlDeclarable From;
    public readonly IUmlDeclarable To;
    public readonly string? Label;

    public UmlSequence(IUmlDeclarable from, IUmlDeclarable to, string? label = null)
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
