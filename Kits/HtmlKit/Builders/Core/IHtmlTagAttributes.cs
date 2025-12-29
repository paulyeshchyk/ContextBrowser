namespace HtmlKit.Builders.Core;

public interface IHtmlTagAttributes : System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>>
{
    void Add(string key, string value);

    void AddIfNotExists(string key, string value);

    void Concat(IHtmlTagAttributes? source);

    string ToString();

    string this[string key] { get; set; }
}
