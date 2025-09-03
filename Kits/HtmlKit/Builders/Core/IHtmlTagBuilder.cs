using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HtmlKit.Builders.Core;

public interface IHtmlTagAttributes : System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>>
{
    void Add(string key, string value);
    void AddIfNotExists(string key, string value);
    void Concat(IHtmlTagAttributes? source);
    string ToString();

    string this[string key] { get; set; }
}

// Класс, который наследуется от Dictionary<string, string> и
// реализует наш новый интерфейс IMyDictionary.
public class HtmlTagAttributes : Dictionary<string, string>, IHtmlTagAttributes
{
    public HtmlTagAttributes() : base()
    {

    }

    public HtmlTagAttributes(StringComparer comparer) : base(comparer)
    {

    }

    public void AddIfNotExists(string key, string value)
    {
        if (!ContainsKey(key))
        {
            this[key] = value;
        }
    }

    public void Concat(IHtmlTagAttributes? source)
    {
        if (source == null)
            return;

        foreach (var pair in source)
        {
            this[pair.Key] = pair.Value;
        }
    }

    public override string ToString()
    {
        if (!this.Any())
        {
            return string.Empty;
        }

        var attrs = this
            .Where(s => !string.IsNullOrWhiteSpace(s.Value))
            .Select(s => $"{s.Key}=\"{s.Value}\"");
        return string.Join(" ", attrs);
    }
}

// pattern: Template method
public interface IHtmlTagBuilder
{
    void Start(TextWriter sb, IHtmlTagAttributes? attrs = null);

    void End(TextWriter sb);
}
