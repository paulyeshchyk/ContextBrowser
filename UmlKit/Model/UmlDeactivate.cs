using UmlKit.Extensions;

namespace UmlKit.Model;

public class UmlDeactivate : IUmlElement, IUmlDeclarable
{
    public readonly string ParticipantName;

    public UmlDeactivate(string participantName)
    {
        ParticipantName = participantName;
    }

    public string Declaration => $"deactivate {this.ParticipantName}";

    public string Alias => ParticipantName.AlphanumericOnly();

    public void WriteTo(TextWriter writer)
    {
        writer.WriteLine(Declaration);
        writer.WriteLine();
    }
}