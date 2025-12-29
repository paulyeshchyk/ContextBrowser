using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ContextBrowserKit.Extensions;

namespace UmlKit.PlantUmlSpecification;

public class UmlParticipant : IUmlParticipant
{
    protected readonly string _raw;

    public UmlParticipantKeyword Keyword { get; set; }

    public string Alias { get; }

    public string FullName => BuildFullName();

    private string BuildFullName()
    {
        var list = new List<string?>();
        list.Add($"\"{_raw}\"");
        if (!string.IsNullOrWhiteSpace(Alias))
        {
            list.Add($"as {Alias}");
        }
        if (!string.IsNullOrWhiteSpace(Url))
        {
            list.Add($"[[.\\{Url}]]");
        }
        var result = string.Join(" ", list.Cast<string>());
        return result;
    }

    public string Declaration => $"{Keyword.ConvertToString()} {this.FullName}";

    public string? Url { get; }

    public UmlParticipant(string raw, string? alias = null, string? url = null, UmlParticipantKeyword keyword = UmlParticipantKeyword.Participant)
    {
        _raw = string.IsNullOrWhiteSpace(raw) ? " " : raw;
        Url = url;
        var theAlias = string.IsNullOrWhiteSpace(alias) ? _raw : alias;
        Alias = string.IsNullOrWhiteSpace(theAlias) ? theAlias : theAlias.AlphanumericOnly(replaceBy: "_");
        Keyword = keyword;
    }

    public void WriteTo(TextWriter writer, UmlWriteOptions writeOptions)
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
