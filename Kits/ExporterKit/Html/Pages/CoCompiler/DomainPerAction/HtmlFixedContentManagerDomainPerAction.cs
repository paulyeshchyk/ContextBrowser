using ContextBrowserKit.Options;
using HtmlKit.Helpers;
using TensorKit.Model;

namespace ExporterKit.Html.Pages.CoCompiler.DomainPerAction;

public class HtmlFixedContentManagerDomainPerAction : IHtmlFixedContentManager
{
    public string TopLeftCell(HtmlTableOptions options) =>
        options.Orientation == TensorPermutationType.Standard
            ? "Action \\ Domain"
            : "Domain \\ Action";

    public string SummaryRow(HtmlTableOptions options) => "<b>Σ</b>";

    public string FirstSummaryRow(HtmlTableOptions options) => "<b>Σ</b>";

    public string LastSummaryRow(HtmlTableOptions options) => "<b>Σ</b>";
}