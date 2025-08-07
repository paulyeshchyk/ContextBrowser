using UmlKit.Extensions;

namespace UmlKit.Model;

public class UmlActivate : IUmlElement, IUmlDeclarable
{
    public readonly string ParticipantName;
    public readonly bool IsSystemCall;

    public UmlActivate(string participantName, bool isSystemCall)
    {
        ParticipantName = participantName;
        IsSystemCall = isSystemCall;
    }

    public string Declaration => $"activate {this.ParticipantName}";

    public string Alias => ParticipantName.AlphanumericOnly();

    public void WriteTo(TextWriter writer)
    {
        writer.WriteLine();

        if(IsSystemCall)
        {
            writer.WriteLine($"->{ParticipantName}");
        }


        writer.WriteLine(Declaration);
    }
}