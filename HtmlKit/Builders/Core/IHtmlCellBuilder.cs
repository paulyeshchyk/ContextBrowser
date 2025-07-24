namespace ContextBrowser.HtmlKit.Builders.Core;

// pattern: Template method
public interface IHtmlCellBuilder
{
    void Cell(TextWriter sb, string? innerHtml = "", string? href = null, string? style = null);
}
