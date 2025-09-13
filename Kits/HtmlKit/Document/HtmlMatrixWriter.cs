using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using ContextBrowserKit.Matrix;
using ContextBrowserKit.Options;
using ContextKit.Model;
using HtmlKit.Builders.Core;
using HtmlKit.Extensions;
using HtmlKit.Helpers;
using HtmlKit.Options;
using HtmlKit.Page;

namespace HtmlKit.Document;

//context: htmlmatrix, build
public class HtmlMatrixWriter : IHtmlMatrixWriter
{
    private readonly IHrefManager _hRefManager;
    private readonly IFixedHtmlContentManager _fixedHtmlContentManager;
    private readonly IHtmlDataCellBuilder<ContextKey> _dataCellBuilder;

    public HtmlMatrixWriter(
        IHtmlDataCellBuilder<ContextKey> dataCellBuilder,
        IHrefManager hrefManager,
        IFixedHtmlContentManager fixedHtmlContentManager)
    {
        _hRefManager = hrefManager;
        _fixedHtmlContentManager = fixedHtmlContentManager;
        _dataCellBuilder = dataCellBuilder;
    }

    public void Write(TextWriter writer, IHtmlMatrix matrix, HtmlMatrixSummary summary, HtmlTableOptions options)
    {
        HtmlBuilderFactory.Table.With(writer, () =>
        {
            WriteHeaderRow(writer, matrix, options);
            WriteSummaryRowIf(writer, matrix, summary, options, SummaryPlacementType.AfterFirst);
            WriteAllDataRows(writer, matrix, summary, options);
            WriteSummaryRowIf(writer, matrix, summary, options, SummaryPlacementType.AfterLast);
        });
    }

    protected void WriteHeaderRow(TextWriter textWriter, IHtmlMatrix matrix, HtmlTableOptions options)
    {
        HtmlBuilderFactory.HtmlBuilderTableRow.Meta.With(textWriter, () =>
        {
            WriteHeaderLeftCorner(textWriter, options);
            WriteHeaderSummaryStart(textWriter, options);
            WriteHeaderCols(textWriter, matrix, options);
            WriteHeaderSummaryEnd(textWriter, options);
        });
    }

    protected void WriteSummaryRowIf(TextWriter textWriter, IHtmlMatrix matrix, HtmlMatrixSummary summary, HtmlTableOptions options, SummaryPlacementType placement)
    {
        if (options.SummaryPlacement == placement)
            WriteSummaryRow(textWriter, matrix, options, summary);
    }

    protected void WriteAllDataRows(TextWriter textWriter, IHtmlMatrix matrix, HtmlMatrixSummary summary, HtmlTableOptions options)
    {
        foreach (var row in matrix.rows)
            WriteDataRow(textWriter, matrix, options, row, summary);
    }

    // Внутренние методы, которые не меняют бизнес-логику, но теперь используют
    // зависимости, переданные в конструкторе.
    internal void WriteHeaderLeftCorner(TextWriter textWriter, HtmlTableOptions options)
    {
        HtmlBuilderFactory.HtmlBuilderTableCell.ActionDomain.With(textWriter, () =>
        {
            var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefSummary(options) }, { "style", "some_special_cell_class" } };
            HtmlBuilderFactory.A.Cell(textWriter, attrs, _fixedHtmlContentManager.TopLeftCell(options), isEncodable: false);
        });
    }

    internal void WriteHeaderSummaryStart(TextWriter textWriter, HtmlTableOptions options)
    {
        if (options.SummaryPlacement == SummaryPlacementType.AfterFirst)
        {
            HtmlBuilderFactory.HtmlBuilderTableCell.SummaryCaption.With(textWriter, () =>
            {
                var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefColHeaderSummary(options) }, { "style", "some_special_cell_class" } };
                HtmlBuilderFactory.A.Cell(textWriter, attrs, _fixedHtmlContentManager.FirstSummaryRow(options), isEncodable: false);
            });
        }
    }

    internal void WriteHeaderCols(TextWriter textWriter, IHtmlMatrix matrix, HtmlTableOptions options)
    {
        foreach (var col in matrix.cols)
        {
            var href = _hRefManager.GetHRefRowMeta(col, options);
            HtmlBuilderFactory.HtmlBuilderTableCell.ColMeta.With(textWriter, () =>
            {
                var attrs = new HtmlTagAttributes() { { "href", href }, { "style", "some_special_cell_class" } };
                HtmlBuilderFactory.A.Cell(textWriter, attrs, col, isEncodable: false);
            });
        }
    }

    internal void WriteHeaderSummaryEnd(TextWriter textWriter, HtmlTableOptions options)
    {
        if (options.SummaryPlacement == SummaryPlacementType.AfterLast)
        {
            HtmlBuilderFactory.HtmlBuilderTableCell.SummaryCaption.With(textWriter, () =>
            {
                var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefRowHeaderSummary(options) }, { "style", "some_special_cell_class" } };
                HtmlBuilderFactory.A.Cell(textWriter, attrs, _fixedHtmlContentManager.LastSummaryRow(options), isEncodable: false);
            });
        }
    }

    internal void WriteSummaryRow(TextWriter textWriter, IHtmlMatrix matrix, HtmlTableOptions options, HtmlMatrixSummary summary)
    {
        var total = summary.ColsSummary?.Values.Sum() ?? 0;

        HtmlBuilderFactory.HtmlBuilderTableRow.Summary.With(textWriter, () =>
        {
            HtmlBuilderFactory.HtmlBuilderTableCell.SummaryCaption.With(textWriter, () =>
            {
                var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefRowHeaderSummaryAfterFirst(options) }, { "style", "some_special_cell_class" } };
                HtmlBuilderFactory.A.Cell(textWriter, attrs, _fixedHtmlContentManager.SummaryRow(options), isEncodable: false);
            });

            if (options.SummaryPlacement == SummaryPlacementType.AfterFirst)
            {
                HtmlBuilderFactory.HtmlBuilderTableCell.TotalSummary.With(textWriter, () =>
                {
                    var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefSummary(options) } };
                    HtmlBuilderFactory.A.Cell(textWriter, attrs, total.ToString(), isEncodable: false);
                });
            }

            foreach (var col in matrix.cols)
            {
                HtmlBuilderFactory.HtmlBuilderTableCell.ColSummary.With(textWriter, () =>
                {
                    var sum = summary.ColsSummary?.GetValueOrDefault(col).ToString() ?? string.Empty;
                    var href = _hRefManager.GetHrefColSummary(col, options);
                    var colCellAttrs = new HtmlTagAttributes() { { "href", href } };
                    HtmlBuilderFactory.A.Cell(textWriter, colCellAttrs, sum, isEncodable: false);
                });
            }

            if (options.SummaryPlacement == SummaryPlacementType.AfterLast)
            {
                HtmlBuilderFactory.HtmlBuilderTableCell.TotalSummary.With(textWriter, () =>
                {
                    var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefSummary(options) } };
                    HtmlBuilderFactory.A.Cell(textWriter, attrs, total.ToString(), isEncodable: false);
                });
            }
        });
    }

    internal void WriteDataRow(TextWriter textWriter, IHtmlMatrix matrix, HtmlTableOptions options, string row, HtmlMatrixSummary summary)
    {
        HtmlBuilderFactory.HtmlBuilderTableRow.Data.With(textWriter, () =>
        {
            HtmlBuilderFactory.HtmlBuilderTableCell.RowMeta.With(textWriter, () =>
            {
                var href = _hRefManager.GetHRefRowHeader(row, options);
                var attrs = new HtmlTagAttributes() { { "href", href } };
                HtmlBuilderFactory.A.Cell(textWriter, attrs, row, isEncodable: false);
            });

            if (options.SummaryPlacement == SummaryPlacementType.AfterFirst)
                WriteRowSummaryCell(textWriter, options, row, summary);

            foreach (var col in matrix.cols)
            {
                var cell = options.Orientation == MatrixOrientationType.ActionRows
                    ? new ContextKey(row, col)
                    : new ContextKey(col, row);

                _dataCellBuilder.BuildDataCell(textWriter, cell, options);
            }

            if (options.SummaryPlacement == SummaryPlacementType.AfterLast)
                WriteRowSummaryCell(textWriter, options, row, summary);
        });
    }

    internal void WriteRowSummaryCell(TextWriter textWriter, HtmlTableOptions options, string row, HtmlMatrixSummary summary)
    {
        HtmlBuilderFactory.HtmlBuilderTableCell.RowSummary.With(textWriter, () =>
        {
            var rowSum = summary.RowsSummary.GetValueOrDefault(row).ToString() ?? string.Empty;
            var attributes = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefRowSummary(row, options) } };
            HtmlBuilderFactory.A.Cell(textWriter, attributes, rowSum, isEncodable: false);
        });
    }
}