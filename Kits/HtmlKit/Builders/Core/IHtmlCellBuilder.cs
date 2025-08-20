namespace HtmlKit.Builders.Core;

// pattern: Template method
public interface IHtmlCellBuilder
{
    void Cell(TextWriter sb, bool plainText, string? innerHtml = "", string? href = null, string? style = null);
}