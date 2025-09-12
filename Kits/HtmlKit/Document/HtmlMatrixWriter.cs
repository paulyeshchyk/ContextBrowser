using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ContextBrowserKit.Matrix;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ExporterKit.Html;
using HtmlKit.Builders.Core;
using HtmlKit.Document.Coverage;
using HtmlKit.Extensions;
using HtmlKit.Helpers;
using HtmlKit.Options;
using HtmlKit.Page;

namespace HtmlKit.Document;

//context: htmlmatrix, build
internal class HtmlMatrixWriter
{
    private readonly IHrefManager _hRefManager;
    private readonly IFixedHtmlContentManager _fixedHtmlContentManager;
    private readonly HtmlTableOptions _options;
    private readonly IHtmlMatrix _matrix;
    private readonly Dictionary<string, int>? _rowsSummary;
    private readonly Dictionary<string, int>? _colsSummary;
    private readonly IContextInfoDataset<ContextInfo> _dataset;
    private readonly IContextKeyIndexer<ContextInfo> _indexer;
    private readonly ICoverageManager _coverageManager;
    private readonly IHtmlPageDataProducer _dataProducer;

    public HtmlMatrixWriter(ICoverageManager coverageManager, IHrefManager hrefManager, IContextInfoDataset<ContextInfo> dataset, IHtmlPageDataProducer dataProducer, IHtmlMatrix matrix, IContextKeyIndexer<ContextInfo> indexer, IFixedHtmlContentManager fixedHtmlContentManager, IHtmlMatrixSummaryBuilder summaryBuilder, HtmlTableOptions options)
    {
        _hRefManager = hrefManager;
        _fixedHtmlContentManager = fixedHtmlContentManager;
        _options = options;
        _dataset = dataset;
        _indexer = indexer;
        _coverageManager = coverageManager;
        _dataProducer = dataProducer;

        _matrix = matrix;
        _rowsSummary = summaryBuilder.RowsSummary(_matrix, dataset, _options.Orientation);
        _colsSummary = summaryBuilder.ColsSummary(_matrix, dataset, _options.Orientation);
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
        foreach (var row in _matrix.rows)
            WriteDataRow(textWriter, row);
        return this;
    }

    //context: htmlmatrix, build
    internal void WriteHeaderLeftCorner(TextWriter textWriter)
    {
        HtmlBuilderFactory.HtmlBuilderTableCell.ActionDomain.With(textWriter, () =>
        {
            var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefSummary(_options) }, { "style", "some_special_cell_class" } };
            HtmlBuilderFactory.A.Cell(textWriter, attrs, _fixedHtmlContentManager.TopLeftCell(_options), isEncodable: false);
        });
    }

    //context: htmlmatrix, build
    internal void WriteHeaderSummaryStart(TextWriter textWriter)
    {
        if (_options.SummaryPlacement == SummaryPlacementType.AfterFirst)
        {
            HtmlBuilderFactory.HtmlBuilderTableCell.SummaryCaption.With(textWriter, () =>
            {
                var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefColHeaderSummary(_options) }, { "style", "some_special_cell_class" } };
                HtmlBuilderFactory.A.Cell(textWriter, attrs, _fixedHtmlContentManager.FirstSummaryRow(_options), isEncodable: false);
            });
        }
    }

    //context: htmlmatrix, build
    internal void WriteHeaderCols(TextWriter textWriter)
    {
        foreach (var col in _matrix.cols)
        {
            var href = _hRefManager.GetHRefRowMeta(col, _options);
            HtmlBuilderFactory.HtmlBuilderTableCell.ColMeta.With(textWriter, () =>
            {
                var attrs = new HtmlTagAttributes() { { "href", href }, { "style", "some_special_cell_class" } };
                HtmlBuilderFactory.A.Cell(textWriter, attrs, col, isEncodable: false);
            });
        }
    }

    //context: htmlmatrix, build
    internal void WriteHeaderSummaryEnd(TextWriter textWriter)
    {
        if (_options.SummaryPlacement == SummaryPlacementType.AfterLast)
        {
            HtmlBuilderFactory.HtmlBuilderTableCell.SummaryCaption.With(textWriter, () =>
            {
                var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefRowHeaderSummary(_options) }, { "style", "some_special_cell_class" } };
                HtmlBuilderFactory.A.Cell(textWriter, attrs, _fixedHtmlContentManager.LastSummaryRow(_options), isEncodable: false);
            });
        }
    }

    //context: htmlmatrix, build
    internal void WriteSummaryRow(TextWriter textWriter)
    {
        var total = _colsSummary?.Values.Sum() ?? 0;

        HtmlBuilderFactory.HtmlBuilderTableRow.Summary.With(textWriter, () =>
        {
            HtmlBuilderFactory.HtmlBuilderTableCell.SummaryCaption.With(textWriter, () =>
            {
                var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefRowHeaderSummaryAfterFirst(_options) }, { "style", "some_special_cell_class" } };
                HtmlBuilderFactory.A.Cell(textWriter, attrs, _fixedHtmlContentManager.SummaryRow(_options), isEncodable: false);
            });

            if (_options.SummaryPlacement == SummaryPlacementType.AfterFirst)
            {
                HtmlBuilderFactory.HtmlBuilderTableCell.TotalSummary.With(textWriter, () =>
                {
                    var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefSummary(_options) } };
                    HtmlBuilderFactory.A.Cell(textWriter, attrs, total.ToString(), isEncodable: false);
                });
            }

            foreach (var col in _matrix.cols)
            {
                HtmlBuilderFactory.HtmlBuilderTableCell.ColSummary.With(textWriter, () =>
                {
                    var sum = _colsSummary?.GetValueOrDefault(col).ToString() ?? string.Empty;
                    var href = _hRefManager.GetHrefColSummary(col, _options);
                    var colCellAttrs = new HtmlTagAttributes() { { "href", href } };
                    HtmlBuilderFactory.A.Cell(textWriter, colCellAttrs, sum, isEncodable: false);
                });
            }

            if (_options.SummaryPlacement == SummaryPlacementType.AfterLast)
            {
                HtmlBuilderFactory.HtmlBuilderTableCell.TotalSummary.With(textWriter, () =>
                {
                    var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefSummary(_options) } };
                    HtmlBuilderFactory.A.Cell(textWriter, attrs, total.ToString(), isEncodable: false);
                });
            }
        });
    }

    //context: htmlmatrix, build
    internal void WriteDataRow(TextWriter textWriter, string row)
    {
        HtmlBuilderFactory.HtmlBuilderTableRow.Data.With(textWriter, () =>
        {
            HtmlBuilderFactory.HtmlBuilderTableCell.RowMeta.With(textWriter, () =>
            {
                var href = _hRefManager.GetHRefRowHeader(row, _options);
                var attrs = new HtmlTagAttributes() { { "href", href } };
                HtmlBuilderFactory.A.Cell(textWriter, attrs, row, isEncodable: false);
            });

            if (_options.SummaryPlacement == SummaryPlacementType.AfterFirst)
                WriteRowSummaryCell(textWriter, row);

            var indexData = _indexer.GetIndexData();

            foreach (var col in _matrix.cols)
            {
                var cell = _options.Orientation == MatrixOrientationType.ActionRows
                    ? new ContextKey(row, col)
                    : new ContextKey(col, row);

                var data = _dataProducer.ProduceData(cell);
                _dataset.TryGetValue(cell, out var methods);

                HtmlBuilderFactory.HtmlBuilderTableCell.Data.With(textWriter, () =>
                {
                    var style = _coverageManager.BuildCellStyle(cell, methods, indexData);
                    var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefCell(cell, _options) } };
                    if (!string.IsNullOrWhiteSpace(style))
                        attrs["style"] = style;
                    HtmlBuilderFactory.A.Cell(textWriter, attrs, data, isEncodable: false);
                });
            }

            if (_options.SummaryPlacement == SummaryPlacementType.AfterLast)
                WriteRowSummaryCell(textWriter, row);
        });
    }

    //context: htmlmatrix, build
    internal void WriteRowSummaryCell(TextWriter _tw, string row)
    {
        HtmlBuilderFactory.HtmlBuilderTableCell.RowSummary.With(_tw, () =>
        {
            var rowSum = _rowsSummary?.GetValueOrDefault(row).ToString() ?? string.Empty;
            var attributes = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefRowSummary(row, _options) } };
            HtmlBuilderFactory.A.Cell(_tw, attributes, rowSum, isEncodable: false);
        });
    }
}