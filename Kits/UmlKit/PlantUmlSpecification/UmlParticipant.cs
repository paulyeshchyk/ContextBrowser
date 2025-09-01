using System;
using System.IO;
using ContextBrowserKit.Extensions;
using UmlKit.Extensions;

namespace UmlKit.Model;

public class UmlParticipant : IUmlParticipant
{
    protected readonly string _raw;

    public UmlParticipantKeyword Keyword { get; set; }

    public string Alias { get; }

    public string FullName => $"\"{_raw}\" as {Alias}";

    public string Declaration => $"{Keyword.ConvertToString()} {this.FullName}";

    public UmlParticipant(string raw, string? alias = null, UmlParticipantKeyword keyword = UmlParticipantKeyword.Participant)
    {
        _raw = string.IsNullOrWhiteSpace(raw) ? " " : raw;
        var theAlias = string.IsNullOrWhiteSpace(alias) ? _raw : alias;
        Alias = string.IsNullOrWhiteSpace(theAlias) ? theAlias : theAlias.AlphanumericOnly(replaceBy: "_");
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
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is null || GetType() != obj.GetType())
        {
            return false;
        }

        UmlParticipant other = (UmlParticipant)obj;
        return _raw.Equals(other._raw) &&
               Keyword == other.Keyword;
    }
}
