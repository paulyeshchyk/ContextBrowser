
using ContextBrowser.UmlKit.Extensions;

namespace ContextBrowser.UmlKit.Model;

public class UmlDeactivate : IUmlElement, IUmlDeclarable
{
    public readonly string ParticipantName;
    public UmlDeactivate(string participantName)
    {
        ParticipantName = participantName;
    }

    public string Declaration => $"deactivate {this.ParticipantName}";

    public string ShortName => ParticipantName.AlphanumericOnly();

    public void WriteTo(TextWriter writer)
    {
        writer.WriteLine(Declaration);
    }
}
