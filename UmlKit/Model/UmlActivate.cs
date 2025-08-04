using ContextBrowser.Infrastructure.Extensions;

namespace UmlKit.Model;

public class UmlActivate : IUmlElement, IUmlDeclarable
{
    public readonly string ParticipantName;

    public UmlActivate(string participantName)
    {
        ParticipantName = participantName;
    }

    public string Declaration => $"activate {this.ParticipantName}";

    public string ShortName => ParticipantName.AlphanumericOnly();

    public void WriteTo(TextWriter writer)
    {
        writer.WriteLine();
        writer.WriteLine(Declaration);
    }
}