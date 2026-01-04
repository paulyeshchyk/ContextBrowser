using ContextBrowserKit.Options;
using ContextKit.ContextData.Naming;
using ContextKit.Model;

namespace HtmlKit.Helpers;

public interface IHtmlHrefManager<TTensor>
    where TTensor : notnull
{
    string GetHrefCell(TTensor cell, HtmlTableOptions _options, INamingProcessor namingProcessor);

    string GetHrefColSummary(ILabeledValue key, HtmlTableOptions _options, INamingProcessor namingProcessor);

    string GetHRefRow(ILabeledValue key, HtmlTableOptions _options, INamingProcessor namingProcessor);

    string GetHRefRowHeader(ILabeledValue key, HtmlTableOptions _options, INamingProcessor namingProcessor);

    string GetHrefRowHeaderSummary(HtmlTableOptions _options, INamingProcessor namingProcessor);

    string GetHRefRowMeta(ILabeledValue key, HtmlTableOptions _options, INamingProcessor namingProcessor);

    string GetHrefRowSummary(ILabeledValue key, HtmlTableOptions _options, INamingProcessor namingProcessor);

    string GetHrefSummary(HtmlTableOptions _options, INamingProcessor namingProcessor);

    string GetHrefColHeaderSummary(HtmlTableOptions _options, INamingProcessor namingProcessor);

    string GetHrefRowHeaderSummaryAfterFirst(HtmlTableOptions _options, INamingProcessor namingProcessor);
}
