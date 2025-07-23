using ContextBrowser.exporter;
using ContextBrowser.Generator.Matrix;
using ContextBrowser.html;
using ContextBrowser.model;
using static ContextBrowser.Generator.Html.IndexGenerator;

namespace ContextBrowser.Generator.Html;

public interface IHtmlPageMatrix
{
    Dictionary<ContextContainer, List<string>> ContextsMatrix { get; }

    UiMatrix UiMatrix { get; }

    Dictionary<string, ContextInfo> ContextsLookup { get; }

    HtmlTableOptions Options { get; }

    ICoverageManager CoverageManager { get; }

    string ProduceData(ContextContainer container);
}

internal class HtmlPageMatrix : HtmlPage, IHtmlPageMatrix
{
    public Dictionary<string, ContextInfo> ContextsLookup { get; }

    public HtmlTableOptions Options { get; }

    public UiMatrix UiMatrix { get; }

    public Dictionary<ContextContainer, List<string>> ContextsMatrix { get; }

    private CoverManager _coverManager = new CoverManager();

    public ICoverageManager CoverageManager => _coverManager;

    public HtmlPageMatrix(UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix, HtmlTableOptions options, Dictionary<string, ContextInfo> contextLookup) : base()
    {
        UiMatrix = uiMatrix;
        ContextsMatrix = matrix;
        Options = options;
        ContextsLookup = contextLookup;
    }

    protected override void WriteContent(TextWriter tw)
    {
        HtmlBuilderFactory.Table.With(tw,() =>
        {
            new HtmlMatrixWriter(this)
                .WriteHeaderRow(tw)
                .WriteSummaryRowIf(tw, SummaryPlacement.AfterFirst)
                .WriteAllDataRows(tw)
                .WriteSummaryRowIf(tw, SummaryPlacement.AfterLast);
        });
    }

    public string ProduceData(ContextContainer container)
    {
        ContextsMatrix.TryGetValue(container, out var methods);
        var cnt = methods?.Count ?? 0;
        return (cnt == 0)
            ? string.Empty
            : cnt.ToString();
    }
}

internal class HtmlMatrixWriter
{
    private IHtmlPageMatrix _htmlPageMatrix;

    public HtmlMatrixWriter(IHtmlPageMatrix htmlPageMatrix)
    {
        _htmlPageMatrix = htmlPageMatrix;
    }

    public HtmlMatrixWriter WriteHeaderRow(TextWriter textWriter)
    {
        HtmlBuilderRow.Meta.With(textWriter,() =>
        {
            WriteHeaderLeftCorner(textWriter);
            WriteHeaderSummaryStart(textWriter);
            WriteHeaderCols(textWriter);
            WriteHeaderSummaryEnd(textWriter);
        });
        return this;
    }

    public HtmlMatrixWriter WriteSummaryRowIf(TextWriter textWriter, SummaryPlacement placement)
    {
        if(_htmlPageMatrix.Options.SummaryPlacement == placement)
            WriteSummaryRow(textWriter);
        return this;
    }

    public HtmlMatrixWriter WriteAllDataRows(TextWriter textWriter)
    {
        foreach(var row in _htmlPageMatrix.UiMatrix.rows)
            WriteDataRow(textWriter, row);
        return this;
    }

    private void WriteHeaderLeftCorner(TextWriter textWriter)
    {
        HtmlBuilderCell.ActionDomain.Cell(textWriter, FixedDataManager.TopLeftCell(_htmlPageMatrix.Options));
    }

    private void WriteHeaderSummaryStart(TextWriter textWriter)
    {
        if(_htmlPageMatrix.Options.SummaryPlacement == SummaryPlacement.AfterFirst)
            HtmlBuilderCell.SummaryCaption.Cell(textWriter, FixedDataManager.FirstSummaryRow(_htmlPageMatrix.Options));
    }

    private void WriteHeaderCols(TextWriter textWriter)
    {
        foreach(var col in _htmlPageMatrix.UiMatrix.cols)
        {
            var href = HrefManager.GetHRefRowMeta(col, _htmlPageMatrix.Options);
            HtmlBuilderCell.ColMeta.Cell(textWriter, col, href);
        }
    }

    private void WriteHeaderSummaryEnd(TextWriter textWriter)
    {
        if(_htmlPageMatrix.Options.SummaryPlacement == SummaryPlacement.AfterLast)
            HtmlBuilderCell.SummaryCaption.Cell(textWriter, FixedDataManager.LastSummaryRow(_htmlPageMatrix.Options));
    }

    private void WriteSummaryRow(TextWriter textWriter)
    {
        var colSums = _htmlPageMatrix.UiMatrix.ColsSummary(_htmlPageMatrix.ContextsMatrix, _htmlPageMatrix.Options.Orientation);
        var total = colSums?.Values.Sum() ?? 0;

        HtmlBuilderRow.Summary.With(textWriter,() =>
        {
            HtmlBuilderCell.SummaryCaption.Cell(textWriter, FixedDataManager.SummaryRow(_htmlPageMatrix.Options));

            if(_htmlPageMatrix.Options.SummaryPlacement == SummaryPlacement.AfterFirst)
                HtmlBuilderCell.TotalSummary.Cell(textWriter, total.ToString(), HrefManager.GetHrefSummary(_htmlPageMatrix.Options));

            foreach(var col in _htmlPageMatrix.UiMatrix.cols)
            {
                var sum = colSums?.GetValueOrDefault(col).ToString() ?? string.Empty;
                var href = HrefManager.GetHrefColSummary(col, _htmlPageMatrix.Options);
                HtmlBuilderCell.ColSummary.Cell(textWriter, sum, href);
            }

            if(_htmlPageMatrix.Options.SummaryPlacement == SummaryPlacement.AfterLast)
                HtmlBuilderCell.TotalSummary.Cell(textWriter, total.ToString(), HrefManager.GetHrefSummary(_htmlPageMatrix.Options));
        });
    }

    private void WriteDataRow(TextWriter textWriter, string row)
    {
        HtmlBuilderRow.Data.With(textWriter,() =>
        {
            var href = HrefManager.GetHRefRowHeader(row, _htmlPageMatrix.Options);
            HtmlBuilderCell.RowMeta.Cell(textWriter, row, href);

            if(_htmlPageMatrix.Options.SummaryPlacement == SummaryPlacement.AfterFirst)
                WriteRowSummaryCell(textWriter, row);

            foreach(var col in _htmlPageMatrix.UiMatrix.cols)
            {
                var cell = _htmlPageMatrix.Options.Orientation == MatrixOrientation.ActionRows
                    ? new ContextContainer(row, col)
                    : new ContextContainer(col, row);


                var data = _htmlPageMatrix.ProduceData(cell);

                var hrefCell = HrefManager.GetHrefCell(cell, _htmlPageMatrix.Options);

                _htmlPageMatrix.ContextsMatrix.TryGetValue(cell, out var methods);
                var style = _htmlPageMatrix.CoverageManager.BuildCellStyle(cell, methods, _htmlPageMatrix.ContextsLookup);

                HtmlBuilderCell.Data.Cell(textWriter, data, hrefCell, style);
            }

            if(_htmlPageMatrix.Options.SummaryPlacement == SummaryPlacement.AfterLast)
                WriteRowSummaryCell(textWriter, row);
        });
    }

    private void WriteRowSummaryCell(TextWriter _tw, string row)
    {
        var rowSum = _htmlPageMatrix.UiMatrix.RowsSummary(_htmlPageMatrix.ContextsMatrix, _htmlPageMatrix.Options.Orientation)?.GetValueOrDefault(row).ToString() ?? string.Empty;
        var href = HrefManager.GetHrefRowSummary(row, _htmlPageMatrix.Options);
        HtmlBuilderCell.RowSummary.Cell(_tw, rowSum, href);
    }
}

public interface ICoverageManager
{
    int GetCoverageValue(ContextInfo? ctx);

    string? BuildCellStyle(ContextContainer cell, List<string>? methods, Dictionary<string, ContextInfo> contextLookup);
}

internal class CoverManager : ICoverageManager
{
    private const string SCssStyleTemplate = "style=\"background-color:{0}; color:black\"";
    private const string SCoverageAttributeName = "coverage";

    public string? BuildCellStyle(ContextContainer cell, List<string>? methods, Dictionary<string, ContextInfo> contextLookup)
    {
        var bgColor = CoverageExts.GetCoverageColorForCell(cell, methods, contextLookup, GetCoverageValue);
        var style = bgColor is null ? null : string.Format(SCssStyleTemplate, bgColor);
        return style;
    }

    public int GetCoverageValue(ContextInfo? ctx)
    {
        if(ctx == null)
            return 0;

        if(!ctx.Dimensions.TryGetValue(SCoverageAttributeName, out var raw))
        {
            return 0;
        }
        return int.TryParse(raw, out var v)
            ? v
            : 0;
    }
}

internal static class FixedDataManager
{
    public static string TopLeftCell(HtmlTableOptions _options) =>
        _options.Orientation == MatrixOrientation.ActionRows
            ? "Action \\ Domain"
            : "Domain \\ Action";

    public static string SummaryRow(HtmlTableOptions _options) => "<b>Σ</b>";

    public static string FirstSummaryRow(HtmlTableOptions _options) => "<b>Σ</b>";

    public static string LastSummaryRow(HtmlTableOptions _options) => "<b>Σ</b>";
}

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

public class HtmlTableOptions
{
    public SummaryPlacement SummaryPlacement = SummaryPlacement.AfterFirst;
    public MatrixOrientation Orientation = MatrixOrientation.DomainRows;
}
