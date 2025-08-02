using ContextBrowser.Infrastructure.Extensions;

namespace UmlKit.Model;

public class UmlParticipant : IUmlElement, IUmlDeclarable
{
    private const string SUnknownParticipant = "unknown_participant";
    protected readonly string _raw;

    public UmlParticipantKeyword Keyword { get; set; }

    public string ShortName => _raw.AlphanumericOnly();

    public string FullName => $"\"{_raw}\" as {_raw.AlphanumericOnly()}";

    public string Declaration => $"{Keyword.ConvertToString()} {this.FullName}";

    public UmlParticipant(string? raw, UmlParticipantKeyword keyword = UmlParticipantKeyword.Participant)
    {
        _raw = string.IsNullOrWhiteSpace(raw) ? SUnknownParticipant : raw;
        Keyword = keyword;
    }

    public void WriteTo(TextWriter writer)
    {
        writer.WriteLine(Declaration);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_raw, Keyword);
    }

    public override bool Equals(object? obj)
    {
        if(ReferenceEquals(this, obj))
        {
            return true;
        }

        if(obj is null || GetType() != obj.GetType())
        {
            return false;
        }

        UmlParticipant other = (UmlParticipant)obj;
        return _raw.Equals(other._raw) &&
               Keyword == other.Keyword;
    }
}

public enum UmlParticipantKeyword
{
    Participant,
    Actor,
    Boundary,
    Control,
    Entity,
    Database,
    Collections,
    Queue
}