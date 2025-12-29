using System;
using System.Collections.Generic;
using System.Linq;

namespace HtmlKit.Builders.Core;

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
