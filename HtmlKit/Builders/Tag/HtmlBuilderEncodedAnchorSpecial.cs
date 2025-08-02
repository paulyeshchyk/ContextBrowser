namespace HtmlKit.Builders.Tag;

// pattern: Builder
public class HtmlBuilderEncodedAnchorSpecial
{
    private readonly string _href;
    private readonly string _text;

    public HtmlBuilderEncodedAnchorSpecial(string href, string? text = "")
    {
        _href = href;
        _text = text ?? string.Empty;
    }

    public override string ToString()
    {
        var Tag = "a";
        return $"<{Tag} href=\"{_href}\" style=\"some_special_cell_class\">{_text}</{Tag}>";
    }
}