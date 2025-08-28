namespace HtmlKit.Builders.Core;

// pattern: Template method
public interface IHtmlTagBuilder
{
    void Start(TextWriter sb);

    void Start(TextWriter writer, string? className, string? id);

    void End(TextWriter sb);
}