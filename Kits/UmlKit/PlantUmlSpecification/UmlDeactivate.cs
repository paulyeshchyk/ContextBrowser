using System.IO;
using ContextBrowserKit.Extensions;

namespace UmlKit.Model;

public class UmlDeactivate : IUmlElement, IUmlDeclarable
{
    public readonly string ParticipantName;

    public UmlDeactivate(string participantName)
    {
        ParticipantName = participantName.AlphanumericOnly(replaceBy: "_");
    }

    public string Declaration => $"deactivate {this.ParticipantName}";

    public string Alias => ParticipantName;

    public void WriteTo(TextWriter writer, int alignNameMaxWidth)
    {
        writer.WriteLine(Declaration);
        writer.WriteLine();
    }
}