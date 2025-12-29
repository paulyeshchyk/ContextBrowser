namespace HtmlKit.Builders.Core;

// using IHtmlTagAttributes = Dictionary<string, string>;

// pattern: Template method
public interface IHtmlBuilder : IHtmlTagBuilder, IHtmlCellBuilder, IHtmlEventHandlerBuilder
{
}