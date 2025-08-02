using System.Net;

namespace HtmlKit.Builders.Tag;

// pattern: Builder
public class HtmlBuilderEncodedAnchor
{
    private readonly string _href;
    private readonly string _text;

    public HtmlBuilderEncodedAnchor(string href, string? text = "")
    {
        _href = WebUtility.HtmlEncode(href);
        _text = WebUtility.HtmlEncode(text ?? string.Empty);
    }

    public override string ToString()
    {
        var Tag = "a";
        return $"<{Tag} href=\"{_href}\">{_text}</{Tag}>";
    }
}