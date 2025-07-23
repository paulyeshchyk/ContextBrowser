using ContextBrowser.exporter;
using ContextBrowser.Generator.Matrix;
using ContextBrowser.model;
using static ContextBrowser.Generator.Html.IndexGenerator;

namespace ContextBrowser.Generator.Html;

// context: html, build
internal class HtmlProducer
{
    public readonly HtmlTableOptions Options;
    private readonly Dictionary<string, ContextInfo> contextLookup;

    public string Title { get; set; } = string.Empty;

    public HtmlProducer(HtmlTableOptions options, Dictionary<string, ContextInfo> contextLookup)
    {
        Options = options;
        this.contextLookup = contextLookup;
    }

    //context: color, ContextInfo, build
    public static string? GetCoverageColorForCell(ContextContainer cell, List<string>? methods, Dictionary<string, ContextInfo> contextLookup, Func<ContextInfo?, int> DimensionValueExtractor)
    {
        if(methods != null && methods.Count > 0)
        {
            var covs = methods
                .Select(name => contextLookup.TryGetValue(name, out var ctx)
                    ? DimensionValueExtractor(ctx)
                    : 0)
                .ToList();

            if(covs.Any())
                return HeatmapColorBuilder.ToHeatmapColor(covs.Average());
        }

        if(contextLookup.TryGetValue(cell.Action, out var actionCtx))
        {
            var aVal = DimensionValueExtractor(actionCtx);
            return HeatmapColorBuilder.ToHeatmapColor(aVal);
        }

        if(contextLookup.TryGetValue(cell.Domain, out var domainCtx))
        {
            var aVal = DimensionValueExtractor(domainCtx);
            return HeatmapColorBuilder.ToHeatmapColor(aVal);
        }

        return null;
    }


    //context: html, build
    protected void WriteHtmlStart(TextWriter sb) => sb.WriteLine("<!DOCTYPE html><html>");

    //context: html, build
    protected void WriteHead(TextWriter sb)
    {
        sb.WriteLine("<head>");
        WriteMeta(sb);
        WriteMetaTitle(sb);
        WriteStyle(sb);
        WriteScript(sb);
        sb.WriteLine("</head>");
    }

    protected virtual void WriteMeta(TextWriter sb)
    {
        sb.WriteLine("<meta charset =\"UTF-8\">");
    }

    protected virtual void WriteScript(TextWriter sb)
    {
        sb.WriteLine("<script>");
        sb.WriteLine(Resources.HtmlProducerContentStyleScript);
        sb.WriteLine("</script>");
    }

    protected virtual void WriteMetaTitle(TextWriter sb)
    {
        sb.WriteLine($"<title>{Title}</title>");
    }

    protected virtual void WriteStyle(TextWriter sb)
    {
        sb.WriteLine("<style>");
        sb.WriteLine(Resources.HtmlProducerContentStyle);
        sb.WriteLine("</style>");
    }

    //context: html, build
    protected void WriteHtmlEnd(TextWriter sb) => sb.WriteLine("</html>");

    //context: html, build
    protected void WriteHtmlBodyStart(TextWriter sb) => sb.WriteLine("<body>");

    //context: html, build
    protected void WriteHtmlBodyEnd(TextWriter sb) => sb.WriteLine("</body>");

    //context: html, build
    protected void WritePageTitle(TextWriter sb) => sb.WriteLine($"<h1>{Title}</h1>");

    //context: html, build
    protected void WriteTableStart(TextWriter sb) => sb.WriteLine("<table>");

    //context: html, build
    protected void WriteTableEnd(TextWriter sb) => sb.WriteLine("</table>");

    //context: html, build
    protected void WriteTableHeaderRow(TextWriter sb, UiMatrix uiMatrix)
    {
        HtmlRowBuilder.Meta.Start(sb);
        ProduceTableHeaderFirstCellContent(sb);
        ProduceTableFirstRowCellSummary(sb);
        ProduceTableHeaderRowCells(sb, uiMatrix);
        ProduceTableLastRowCellSummary(sb);
        HtmlRowBuilder.Meta.End(sb);
    }

    //context: html, build
    protected void WriteColumnSummaryRow(TextWriter sb, UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix)
    {
        var colSums = uiMatrix.ColsSummary(matrix, Options.Orientation);
        var totalSum = colSums?.Values.Sum() ?? 0;

        HtmlRowBuilder.Summary.Start(sb);
        HtmlCellBuilder.SummaryCaption.Cell(sb, "<b>Σ</b>");

        if(Options.SummaryPlacement == SummaryPlacement.AfterFirst)
            HtmlCellBuilder.TotalSummary.Cell(sb, totalSum.ToString(), "index.html");

        ProduceColumnSummaryCells(sb, uiMatrix, colSums);

        if(Options.SummaryPlacement == SummaryPlacement.AfterLast)
            HtmlCellBuilder.TotalSummary.Cell(sb, totalSum.ToString(), "index.html");

        HtmlRowBuilder.Summary.End(sb);
    }

    //context: html, build
    protected void ProduceDataRow(TextWriter sb, string row, UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix)
    {
        HtmlRowBuilder.Data.Start(sb);
        ProduceRowHeaderCell(sb, row);

        if(Options.SummaryPlacement == SummaryPlacement.AfterFirst)
            ProduceRowSummaryCell(sb, row, uiMatrix, matrix);

        ProduceDataCells(sb, row, uiMatrix, matrix);
        ProduceRowSummaryCellLast(sb, row, uiMatrix, matrix);

        HtmlRowBuilder.Data.End(sb);
    }

    protected void Produce(TextWriter sb, UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix)
    {
        WriteHtmlStart(sb);
        WriteHead(sb);
        WriteHtmlBodyStart(sb);
        WritePageTitle(sb);
        WriteTableStart(sb);
        WriteMatrix(sb, uiMatrix, matrix);
        WriteTableEnd(sb);
        WriteHtmlBodyEnd(sb);
        WriteHtmlEnd(sb);
    }

    public string ToHtmlString(UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix)
    {
        using var sw = new StringWriter();
        Produce(sw, uiMatrix, matrix);
        return sw.ToString();
    }


    //context: html, build
    protected void WriteMatrix(TextWriter sb, UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix)
    {
        WriteTableHeaderRow(sb, uiMatrix);

        if(Options.SummaryPlacement == SummaryPlacement.AfterFirst)
            WriteColumnSummaryRow(sb, uiMatrix, matrix);

        foreach(var row in uiMatrix.rows)
            ProduceDataRow(sb, row, uiMatrix, matrix);

        if(Options.SummaryPlacement == SummaryPlacement.AfterLast)
            WriteColumnSummaryRow(sb, uiMatrix, matrix);
    }

    private void ProduceTableHeaderFirstCellContent(TextWriter sb)
    {
        var theText = Options.Orientation == MatrixOrientation.ActionRows
            ? "Action \\ Domain"
            : "Domain \\ Action";
        HtmlCellBuilder.ActionDomain.Cell(sb, theText);
    }

    private void ProduceTableFirstRowCellSummary(TextWriter sb)
    {
        if(Options.SummaryPlacement == SummaryPlacement.AfterFirst)
        {
            HtmlCellBuilder.SummaryCaption.Cell(sb, "<b>Σ</b>");
        }
    }

    private void ProduceTableLastRowCellSummary(TextWriter sb)
    {
        if(Options.SummaryPlacement == SummaryPlacement.AfterLast)
        {
            HtmlCellBuilder.SummaryCaption.Cell(sb, "<b>Σ</b>");
        }
    }

    private void ProduceTableHeaderRowCells(TextWriter sb, UiMatrix uiMatrix)
    {
        foreach(var metaName in uiMatrix.cols)
        {
            var hRef = Options.Orientation == MatrixOrientation.ActionRows
                ? $"domain_{metaName}.html"
                : $"action_{metaName}.html";

            HtmlCellBuilder.ColMeta.Cell(sb, metaName, hRef);
        }
    }

    private void ProduceRowHeaderCell(TextWriter sb, string metaName)
    {
        var hRef = Options.Orientation == MatrixOrientation.ActionRows
            ? $"action_{metaName}.html"
            : $"domain_{metaName}.html";

        HtmlCellBuilder.RowMeta.Cell(sb, metaName, hRef);
    }

    private void ProduceColumnSummaryCells(TextWriter sb, UiMatrix uiMatrix, Dictionary<string, int>? colSums)
    {
        foreach(var metaName in uiMatrix.cols)
        {
            var sum = colSums?[metaName];
            var href = Options.Orientation == MatrixOrientation.ActionRows
                ? $"domain_{metaName}.html"
                : $"action_{metaName}.html";

            HtmlCellBuilder.ColSummary.Cell(sb, sum?.ToString() ?? string.Empty, href);
        }
    }

    private void ProduceRowSummaryCell(TextWriter sb, string row, UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix)
    {
        var hRef = Options.Orientation == MatrixOrientation.ActionRows
            ? $"action_{row}.html"
            : $"domain_{row}.html";

        var rowSum = uiMatrix.RowsSummary(matrix, Options.Orientation)?[row];
        HtmlCellBuilder.RowSummary.Cell(sb, rowSum?.ToString() ?? string.Empty, hRef);
    }

    private static int GetCoverageValue(ContextInfo? ctx)
    {
        return ctx?.GetDimensionIntValue("coverage") ?? 0;
    }

    private void ProduceDataCells(TextWriter sb, string row, UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix)
    {
        foreach(var col in uiMatrix.cols)
        {
            var cell = Options.Orientation == MatrixOrientation.ActionRows
                ? new ContextContainer(row, col)
                : new ContextContainer(col, row);

            var hasMethods = matrix.TryGetValue(cell, out var methods) && methods.Any();
            var hRef = $"composite_{cell.Action}_{cell.Domain}.html";

            string style = string.Empty;
            var bgColor = GetCoverageColorForCell(cell, hasMethods ? methods : null, contextLookup, GetCoverageValue);
            if(bgColor != null)
            {
                style = $" style=\"inherited;background-color:{bgColor}; color:black\"";
            }
            HtmlCellBuilder.Data.Cell(sb, $"{(hasMethods ? methods?.Count ?? 0 : "&nbsp;")}", hRef, style);
        }
    }

    private void ProduceRowSummaryCellLast(TextWriter sb, string row, UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix)
    {
        if(Options.SummaryPlacement != SummaryPlacement.AfterLast)
            return;

        var rowFile = Options.Orientation == MatrixOrientation.ActionRows
            ? $"action_{row}.html"
            : $"domain_{row}.html";

        var rowSum = uiMatrix.RowsSummary(matrix, Options.Orientation)?[row];
        HtmlCellBuilder.RowSummary.Cell(sb, rowSum?.ToString() ?? string.Empty, rowFile);
    }
}

internal class HtmlTableOptions
{
    public SummaryPlacement SummaryPlacement = SummaryPlacement.AfterFirst;
    public MatrixOrientation Orientation = MatrixOrientation.DomainRows;
}
