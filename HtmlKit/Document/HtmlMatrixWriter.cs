using ContextBrowser.ContextKit.Matrix;
using ContextBrowser.ContextKit.Model;
using ContextBrowser.HtmlKit.Builders.Core;
using ContextBrowser.HtmlKit.Builders.Rows;
using ContextBrowser.HtmlKit.Builders.Tag;
using ContextBrowser.HtmlKit.Helpers;
using ContextBrowser.HtmlKit.Model;

namespace ContextBrowser.HtmlKit.Document;

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
        HtmlTagBuilderFactory.ActionDomain.Cell(textWriter, FixedDataManager.TopLeftCell(_htmlPageMatrix.Options));
    }

    private void WriteHeaderSummaryStart(TextWriter textWriter)
    {
        if(_htmlPageMatrix.Options.SummaryPlacement == SummaryPlacement.AfterFirst)
            HtmlTagBuilderFactory.SummaryCaption.Cell(textWriter, FixedDataManager.FirstSummaryRow(_htmlPageMatrix.Options));
    }

    private void WriteHeaderCols(TextWriter textWriter)
    {
        foreach(var col in _htmlPageMatrix.UiMatrix.cols)
        {
            var href = HrefManager.GetHRefRowMeta(col, _htmlPageMatrix.Options);
            HtmlTagBuilderFactory.ColMeta.Cell(textWriter, col, href);
        }
    }

    private void WriteHeaderSummaryEnd(TextWriter textWriter)
    {
        if(_htmlPageMatrix.Options.SummaryPlacement == SummaryPlacement.AfterLast)
            HtmlTagBuilderFactory.SummaryCaption.Cell(textWriter, FixedDataManager.LastSummaryRow(_htmlPageMatrix.Options));
    }

    private void WriteSummaryRow(TextWriter textWriter)
    {
        var colSums = _htmlPageMatrix.UiMatrix.ColsSummary(_htmlPageMatrix.ContextsMatrix, _htmlPageMatrix.Options.Orientation);
        var total = colSums?.Values.Sum() ?? 0;

        HtmlBuilderRow.Summary.With(textWriter,() =>
        {
            HtmlTagBuilderFactory.SummaryCaption.Cell(textWriter, FixedDataManager.SummaryRow(_htmlPageMatrix.Options));

            if(_htmlPageMatrix.Options.SummaryPlacement == SummaryPlacement.AfterFirst)
                HtmlTagBuilderFactory.TotalSummary.Cell(textWriter, total.ToString(), HrefManager.GetHrefSummary(_htmlPageMatrix.Options));

            foreach(var col in _htmlPageMatrix.UiMatrix.cols)
            {
                var sum = colSums?.GetValueOrDefault(col).ToString() ?? string.Empty;
                var href = HrefManager.GetHrefColSummary(col, _htmlPageMatrix.Options);
                HtmlTagBuilderFactory.ColSummary.Cell(textWriter, sum, href);
            }

            if(_htmlPageMatrix.Options.SummaryPlacement == SummaryPlacement.AfterLast)
                HtmlTagBuilderFactory.TotalSummary.Cell(textWriter, total.ToString(), HrefManager.GetHrefSummary(_htmlPageMatrix.Options));
        });
    }

    private void WriteDataRow(TextWriter textWriter, string row)
    {
        HtmlBuilderRow.Data.With(textWriter,() =>
        {
            var href = HrefManager.GetHRefRowHeader(row, _htmlPageMatrix.Options);
            HtmlTagBuilderFactory.RowMeta.Cell(textWriter, row, href);

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

                HtmlTagBuilderFactory.Data.Cell(textWriter, data, hrefCell, style);
            }

            if(_htmlPageMatrix.Options.SummaryPlacement == SummaryPlacement.AfterLast)
                WriteRowSummaryCell(textWriter, row);
        });
    }

    private void WriteRowSummaryCell(TextWriter _tw, string row)
    {
        var rowSum = _htmlPageMatrix.UiMatrix.RowsSummary(_htmlPageMatrix.ContextsMatrix, _htmlPageMatrix.Options.Orientation)?.GetValueOrDefault(row).ToString() ?? string.Empty;
        var href = HrefManager.GetHrefRowSummary(row, _htmlPageMatrix.Options);
        HtmlTagBuilderFactory.RowSummary.Cell(_tw, rowSum, href);
    }
}
