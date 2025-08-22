using ContextBrowserKit.Options;
using ContextKit.Model.Matrix;
using HtmlKit.Builders.Core;
using HtmlKit.Extensions;
using HtmlKit.Helpers;
using HtmlKit.Options;
using HtmlKit.Page;

namespace HtmlKit.Document;

//context: htmlmatrix, build
internal class HtmlMatrixWriter
{
    private IHtmlPageMatrix _htmlPageMatrix;
    private HtmlTableOptions _options;

    public HtmlMatrixWriter(IHtmlPageMatrix htmlPageMatrix, HtmlTableOptions options)
    {
        _htmlPageMatrix = htmlPageMatrix;
        _options = options;
    }

    //context: htmlmatrix, build
    public HtmlMatrixWriter WriteHeaderRow(TextWriter textWriter)
    {
        HtmlBuilderFactory.HtmlBuilderTableRow.Meta.With(textWriter, () =>
        {
            WriteHeaderLeftCorner(textWriter);
            WriteHeaderSummaryStart(textWriter);
            WriteHeaderCols(textWriter);
            WriteHeaderSummaryEnd(textWriter);
        });
        return this;
    }

    //context: htmlmatrix, build
    public HtmlMatrixWriter WriteSummaryRowIf(TextWriter textWriter, SummaryPlacementType placement)
    {
        if (_options.SummaryPlacement == placement)
            WriteSummaryRow(textWriter);
        return this;
    }

    //context: htmlmatrix, build
    public HtmlMatrixWriter WriteAllDataRows(TextWriter textWriter)
    {
        foreach (var row in _htmlPageMatrix.UiMatrix.rows)
            WriteDataRow(textWriter, row);
        return this;
    }

    //context: htmlmatrix, build
    internal void WriteHeaderLeftCorner(TextWriter textWriter)
    {
        HtmlBuilderFactory.HtmlBuilderTableCell.ActionDomain.Cell(textWriter, FixedDataManager.TopLeftCell(_options), HrefManager.GetHrefSummary(_options));
    }

    //context: htmlmatrix, build
    internal void WriteHeaderSummaryStart(TextWriter textWriter)
    {
        if (_options.SummaryPlacement == SummaryPlacementType.AfterFirst)
            HtmlBuilderFactory.HtmlBuilderTableCell.SummaryCaption.Cell(textWriter, FixedDataManager.FirstSummaryRow(_options), HrefManager.GetHrefColHeaderSummary(_options));
    }

    //context: htmlmatrix, build
    internal void WriteHeaderCols(TextWriter textWriter)
    {
        foreach (var col in _htmlPageMatrix.UiMatrix.cols)
        {
            var href = HrefManager.GetHRefRowMeta(col, _options);
            HtmlBuilderFactory.HtmlBuilderTableCell.ColMeta.Cell(textWriter, col, href);
        }
    }

    //context: htmlmatrix, build
    internal void WriteHeaderSummaryEnd(TextWriter textWriter)
    {
        if (_options.SummaryPlacement == SummaryPlacementType.AfterLast)
            HtmlBuilderFactory.HtmlBuilderTableCell.SummaryCaption.Cell(textWriter, FixedDataManager.LastSummaryRow(_options), HrefManager.GetHrefRowHeaderSummary(_options));
    }

    //context: htmlmatrix, build
    internal void WriteSummaryRow(TextWriter textWriter)
    {
        var colSums = _htmlPageMatrix.UiMatrix.ColsSummary(_htmlPageMatrix.ContextsMatrix, _options.Orientation);
        var total = colSums?.Values.Sum() ?? 0;

        HtmlBuilderFactory.HtmlBuilderTableRow.Summary.With(textWriter, () =>
        {
            HtmlBuilderFactory.HtmlBuilderTableCell.SummaryCaption.Cell(textWriter, FixedDataManager.SummaryRow(_options), HrefManager.GetHrefRowHeaderSummaryAfterFirst(_options));

            if (_options.SummaryPlacement == SummaryPlacementType.AfterFirst)
                HtmlBuilderFactory.HtmlBuilderTableCell.TotalSummary.Cell(textWriter, total.ToString(), HrefManager.GetHrefSummary(_options));

            foreach (var col in _htmlPageMatrix.UiMatrix.cols)
            {
                var sum = colSums?.GetValueOrDefault(col).ToString() ?? string.Empty;
                var href = HrefManager.GetHrefColSummary(col, _options);
                HtmlBuilderFactory.HtmlBuilderTableCell.ColSummary.Cell(textWriter, sum, href);
            }

            if (_options.SummaryPlacement == SummaryPlacementType.AfterLast)
                HtmlBuilderFactory.HtmlBuilderTableCell.TotalSummary.Cell(textWriter, total.ToString(), HrefManager.GetHrefSummary(_options));
        });
    }

    //context: htmlmatrix, build
    internal void WriteDataRow(TextWriter textWriter, string row)
    {
        HtmlBuilderFactory.HtmlBuilderTableRow.Data.With(textWriter, () =>
        {
            var href = HrefManager.GetHRefRowHeader(row, _options);
            HtmlBuilderFactory.HtmlBuilderTableCell.RowMeta.Cell(textWriter, row, href);

            if (_options.SummaryPlacement == SummaryPlacementType.AfterFirst)
                WriteRowSummaryCell(textWriter, row);

            foreach (var col in _htmlPageMatrix.UiMatrix.cols)
            {
                var cell = _options.Orientation == MatrixOrientationType.ActionRows
                    ? new ContextInfoMatrixCell(row, col)
                    : new ContextInfoMatrixCell(col, row);

                var data = _htmlPageMatrix.ProduceData(cell);
                var hrefCell = HrefManager.GetHrefCell(cell, _options);
                _htmlPageMatrix.ContextsMatrix.TryGetValue(cell, out var methods);
                var style = _htmlPageMatrix.CoverageManager.BuildCellStyle(cell, methods, _htmlPageMatrix.ContextsLookup);

                HtmlBuilderFactory.HtmlBuilderTableCell.Data.Cell(textWriter, data, hrefCell, style);
            }

            if (_options.SummaryPlacement == SummaryPlacementType.AfterLast)
                WriteRowSummaryCell(textWriter, row);
        });
    }

    //context: htmlmatrix, build
    internal void WriteRowSummaryCell(TextWriter _tw, string row)
    {
        var rowSum = _htmlPageMatrix.UiMatrix.RowsSummary(_htmlPageMatrix.ContextsMatrix, _options.Orientation)?.GetValueOrDefault(row).ToString() ?? string.Empty;
        var href = HrefManager.GetHrefRowSummary(row, _options);
        HtmlBuilderFactory.HtmlBuilderTableCell.RowSummary.Cell(_tw, rowSum, href);
    }
}