using ContextBrowser.ContextKit.Matrix;
using ContextBrowser.ContextKit.Model;
using ContextBrowser.HtmlKit.Model;

namespace ContextBrowser.HtmlKit.Helpers;

internal static class HrefManager
{
    public static string GetHrefColSummary(string key, HtmlTableOptions _options) =>
        _options.Orientation == MatrixOrientation.ActionRows
            ? $"domain_{key}.html"
            : $"action_{key}.html";

    public static string GetHrefRowSummary(string key, HtmlTableOptions _options) =>
        _options.Orientation == MatrixOrientation.ActionRows
            ? $"action_{key}.html"
            : $"domain_{key}.html";

    public static string GetHRefRow(string key, HtmlTableOptions _options) =>
        _options.Orientation == MatrixOrientation.ActionRows
            ? $"action_{key}.html"
            : $"domain_{key}.html";

    public static string GetHRefRowMeta(string key, HtmlTableOptions _options) =>
        _options.Orientation == MatrixOrientation.ActionRows
            ? $"domain_{key}.html"
            : $"action_{key}.html";

    public static string GetHRefRowHeader(string key, HtmlTableOptions _options) =>
        _options.Orientation == MatrixOrientation.ActionRows
            ? $"action_{key}.html"
            : $"domain_{key}.html";

    public static string GetHrefCell(ContextContainer cell, HtmlTableOptions _options) =>
        $"composite_{cell.Action}_{cell.Domain}.html";

    public static string GetHrefSummary(HtmlTableOptions _options) =>
        $"index.html";
}
