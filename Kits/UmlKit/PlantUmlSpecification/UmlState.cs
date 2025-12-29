using System;
using System.IO;
using ContextBrowserKit.Extensions;

namespace UmlKit.PlantUmlSpecification;

public class UmlState : IUmlParticipant
{
    private const string SUnknownState = "unknown_state";
    protected readonly string _raw;

    public string Alias => _raw.AlphanumericOnly();

    public string FullName => $"\"{_raw}\" as {_raw.AlphanumericOnly()}";

    public string Declaration => $"state {this.FullName}";

    public string? Url { get; }

    public UmlState(string? raw, string? alias, string? url = null)
    {
        _raw = string.IsNullOrWhiteSpace(raw) ? SUnknownState : raw;
        Url = url;
    }

    public void WriteTo(TextWriter writer, UmlWriteOptions writeOptions)
    {
        writer.WriteLine();
        writer.WriteLine(Declaration);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_raw);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is null || GetType() != obj.GetType())
        {
            return false;
        }

        UmlState other = (UmlState)obj;
        return _raw.Equals(other._raw);
    }
}

public static class UmlStateFactory
{
    public static UmlState AsterixState = new UmlState("[*]", null);
}
