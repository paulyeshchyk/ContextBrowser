using ContextBrowser.Generator.Matrix;
using ContextBrowser.Parser;
using System.Text;
using static ContextBrowser.Generator.Html.IndexGenerator;

namespace ContextBrowser.Generator.Html;

internal class HtmlProducer
{
    private readonly StringBuilder sb = new StringBuilder();
    public readonly HtmlTableOptions Options;
    private readonly Dictionary<string, ContextInfo> contextLookup;

    public HtmlProducer(HtmlTableOptions options, Dictionary<string, ContextInfo> contextLookup)
    {
        Options = options;
        this.contextLookup = contextLookup;
    }

    // context: html, read
    public string GetResult() => sb.ToString();

    // context: html, build, doctype
    public void ProduceHtmlStart() => sb.AppendLine("<!DOCTYPE html><html>");

    // context: html, build, header
    public void ProduceHead()
    {
        sb.AppendLine("<head><meta charset=\"UTF-8\"><title>📦 Контекстная матрица</title>");
        sb.AppendLine("<style>");
        sb.AppendLine(Resources.HtmlProducerContentStyle);
        sb.AppendLine("</style>");
        sb.AppendLine("<script>");
        sb.AppendLine(Resources.HtmlProducerContentStyleScript);
        sb.AppendLine("</script>");
        sb.AppendLine("</head>");
    }

    // context: html, build
    public void ProduceHtmlEnd() => sb.AppendLine("</html>");

    // context: html, build
    public void ProduceHtmlBodyStart() => sb.AppendLine("<body>");

    // context: html, build
    public void ProduceHtmlBodyEnd() => sb.AppendLine("</body>");

    // context: html, build
    public void ProduceTitle() => sb.AppendLine("<h1>📦 Навигация по архитектурным зонам</h1>");

    // context: html, build
    public void ProduceTableStart() => sb.AppendLine("<table>");

    // context: html, build
    public void ProduceTableEnd() => sb.AppendLine("</table>");

    // context: html, build
    public void ProduceTableHeaderRow(UiMatrix uiMatrix)
    {
        sb.Append("<tr>");
        ProduceTableHeaderFirstCellContent();
        ProduceTableFirstRowCellSummary();
        ProduceTableHeaderRowCells(uiMatrix);
        ProduceTableLastRowCellSummary();
        sb.AppendLine("</tr>");
    }

    // context: html, build
    public void ProduceColumnSummaryRow(UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix)
    {
        var colSums = uiMatrix.ColsSummary(matrix, Options.Orientation);
        var totalSum = colSums?.Values.Sum() ?? 0;

        sb.Append("<tr>");
        sb.Append("<td><b>Σ</b></td>");

        if(Options.SummaryPlacement == SummaryPlacement.AfterFirst)
            sb.Append($"<td><a href=\"index.html\">{totalSum}</a></td>");

        ProduceColumnSummaryCells(uiMatrix, colSums);

        if(Options.SummaryPlacement == SummaryPlacement.AfterLast)
            sb.Append($"<td><a href=\"index.html\">{totalSum}</a></td>");

        sb.AppendLine("</tr>");
    }

    // context: html, read
    public static string? GetCoverageColorForCell(ContextContainer cell, List<string>? methods, Dictionary<string, ContextInfo> contextLookup, Func<ContextInfo?, int> DimensionValueExtractor)
    {
        // 1) Если в ячейке есть методы — усредняем их coverage
        if(methods != null && methods.Count > 0)
        {
            var covs = methods
                .Select(name => contextLookup.TryGetValue(name, out var ctx)
                    ? DimensionValueExtractor(ctx)
                    : 0
                    )
                .ToList();

            if(covs.Any())
                return HeatmapColorBuilder.ToHeatmapColor(covs.Average());
        }

        // 2) Нет методов или у методов нет coverage — пробуем action
        if(contextLookup.TryGetValue(cell.Action, out var actionCtx))
        {
            var aVal = DimensionValueExtractor(actionCtx);
            return HeatmapColorBuilder.ToHeatmapColor(aVal);
        }

        // 3) Пробуем domain
        if(contextLookup.TryGetValue(cell.Domain, out var domainCtx))
        {
            var aVal = DimensionValueExtractor(domainCtx);
            return HeatmapColorBuilder.ToHeatmapColor(aVal);
        }

        return null;
    }

    // context: html, build
    public void ProduceDataRow(string row, UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix)
    {
        sb.Append("<tr>");
        ProduceRowHeaderCell(row);

        if(Options.SummaryPlacement == SummaryPlacement.AfterFirst)
            ProduceRowSummaryCell(row, uiMatrix, matrix);

        ProduceDataCells(row, uiMatrix, matrix);

        ProduceRowSummaryCellLast(row, uiMatrix, matrix);

        sb.AppendLine("</tr>");
    }

    // context: html, build, matrix
    public void ProduceMatrix(UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix)
    {
        ProduceTableHeaderRow(uiMatrix);

        if(Options.SummaryPlacement == SummaryPlacement.AfterFirst)
            ProduceColumnSummaryRow(uiMatrix, matrix);

        foreach(var row in uiMatrix.rows)
            ProduceDataRow(row, uiMatrix, matrix);

        if(Options.SummaryPlacement == SummaryPlacement.AfterLast)
            ProduceColumnSummaryRow(uiMatrix, matrix);
    }

    private void ProduceTableHeaderFirstCellContent() => sb.AppendLine($"<th><a href=\"\">{(Options.Orientation == MatrixOrientation.ActionRows ? "Action \\ Domain" : "Domain \\ Action")}</a></th>");

    private void ProduceTableFirstRowCellSummary()
    {
        if(Options.SummaryPlacement == SummaryPlacement.AfterFirst)
            sb.Append("<th><b>Σ</b></th>");
    }

    private void ProduceTableLastRowCellSummary()
    {
        if(Options.SummaryPlacement == SummaryPlacement.AfterLast)
            sb.Append("<th><b>Σ</b></th>");
    }

    private void ProduceTableHeaderRowCells(UiMatrix uiMatrix)
    {
        foreach(var metaName in uiMatrix.cols)
        {
            var hRef = Options.Orientation == MatrixOrientation.ActionRows
                ? $"domain_{metaName}.html"
                : $"action_{metaName}.html";

            sb.Append($"<th><a href=\"{hRef}\">{metaName}</a></th>");
        }
    }

    private void ProduceRowHeaderCell(string metaName)
    {
        var hRef = Options.Orientation == MatrixOrientation.ActionRows
            ? $"action_{metaName}.html"
            : $"domain_{metaName}.html";

        sb.Append($"<td><a href=\"{hRef}\">{metaName}</a></td>");
    }

    private void ProduceColumnSummaryCells(UiMatrix uiMatrix, Dictionary<string, int>? colSums)
    {
        foreach(var metaName in uiMatrix.cols)
        {
            var sum = colSums?[metaName];
            var href = Options.Orientation == MatrixOrientation.ActionRows
                ? $"domain_{metaName}.html"
                : $"action_{metaName}.html";

            sb.Append($"<td><a href=\"{href}\">{sum}</a></td>");
        }
    }

    private void ProduceRowSummaryCell(string row, UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix)
    {
        var hRef = Options.Orientation == MatrixOrientation.ActionRows
            ? $"action_{row}.html"
            : $"domain_{row}.html";

        var rowSum = uiMatrix.RowsSummary(matrix, Options.Orientation)?[row];
        sb.Append($"<td><a href=\"{hRef}\">{rowSum}</a></td>");
    }

    private static int GetCoverageValue(ContextInfo? ctx)
    {
        return ctx?.GetDimensionIntValue("coverage") ?? 0;
    }

    private void ProduceDataCells(string row, UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix)
    {
        foreach(var col in uiMatrix.cols)
        {
            // определяем cell с учётом ориентации
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

            sb.Append("<td")
              .Append(style)
              .Append($"><a href=\"{hRef}\">{(hasMethods ? methods?.Count ?? 0 : "&nbsp;")}</a>")
              .AppendLine("</td>");
        }
    }

    private void ProduceRowSummaryCellLast(string row, UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix)
    {
        if(Options.SummaryPlacement != SummaryPlacement.AfterLast)
            return;

        var rowFile = Options.Orientation == MatrixOrientation.ActionRows
            ? $"action_{row}.html"
            : $"domain_{row}.html";

        var rowSum = uiMatrix.RowsSummary(matrix, Options.Orientation)?[row];
        sb.Append($"<td><a href=\"{rowFile}\">{rowSum}</a></td>");
    }
}

internal class HtmlTableOptions
{
    public SummaryPlacement SummaryPlacement = SummaryPlacement.AfterFirst;
    public MatrixOrientation Orientation = MatrixOrientation.DomainRows;
}
