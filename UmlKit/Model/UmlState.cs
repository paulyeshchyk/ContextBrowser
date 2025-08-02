using ContextBrowser.Infrastructure.Extensions;

namespace UmlKit.Model;

public class UmlState : IUmlElement, IUmlDeclarable
{
    private const string SUnknownState = "unknown_state";
    protected readonly string _raw;

    public string ShortName => _raw.AlphanumericOnly();

    public string FullName => $"\"{_raw}\" as {_raw.AlphanumericOnly()}";

    public string Declaration => $"state {this.FullName}";

    public UmlState(string? raw)
    {
        _raw = string.IsNullOrWhiteSpace(raw) ? SUnknownState : raw;
    }

    public void WriteTo(TextWriter writer)
    {
        writer.WriteLine(Declaration);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_raw);
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

        UmlState other = (UmlState)obj;
        return _raw.Equals(other._raw);
    }
}

public static class UmlStateFactory
{
    public static UmlState AsterixState = new UmlState("[*]");
}