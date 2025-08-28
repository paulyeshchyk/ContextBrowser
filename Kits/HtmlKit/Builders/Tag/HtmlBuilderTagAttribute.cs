namespace HtmlKit.Builders.Tag;

// pattern: Builder
public static class HtmlBuilderTagAttribute
{
    public static string BuildAttributes(IDictionary<string, string?>? attributes)
    {
        if (attributes == null || attributes.Count == 0)
        {
            return string.Empty;
        }

        var sb = new System.Text.StringBuilder();
        foreach (var kvp in attributes)
        {
            if (!string.IsNullOrWhiteSpace(kvp.Key) && !string.IsNullOrWhiteSpace(kvp.Value))
            {
                sb.Append($" {kvp.Key}=\"{kvp.Value}\"");
            }
        }
        return sb.ToString();
    }

    // Overload for simpler common attributes like class/style
    public static string BuildClassAttribute(string? className)
    {
        return string.IsNullOrWhiteSpace(className) ? string.Empty : $" class=\"{className}\"";
    }

    public static string BuildIdAttribute(string? idValue)
    {
        return string.IsNullOrWhiteSpace(idValue) ? string.Empty : $" id=\"{idValue}\"";
    }

    public static string BuildStyleAttribute(string? style)
    {
        return string.IsNullOrWhiteSpace(style) ? string.Empty : $" {style}";
    }

    public static string BuildOnClickAttribute(string? onClickEvent)
    {
        return string.IsNullOrWhiteSpace(onClickEvent) ? string.Empty : $" onClick = \"{onClickEvent}\"";
    }
}