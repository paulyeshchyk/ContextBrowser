using ContextBrowser.ContextKit.Matrix;
using ContextBrowser.ContextKit.Model;
using ContextBrowser.HtmlKit.Document;
using ContextBrowser.HtmlKit.Model;

namespace ContextBrowser.HtmlKit.Exporter;

//context: build, html
public static partial class IndexGenerator
{
    //context: build, html
    public static void GenerateContextIndexHtml(Dictionary<ContextContainer, List<string>> matrix, Dictionary<string, ContextInfo> allContextInfo, string outputFile, UnclassifiedPriority priority = UnclassifiedPriority.None, MatrixOrientation orientation = MatrixOrientation.DomainRows, SummaryPlacement summaryPlacement = SummaryPlacement.AfterFirst)
    {
        var uiMatrix = UiMatrixGenerator.Generate(matrix, orientation, priority);
        var options = new HtmlTableOptions { SummaryPlacement = summaryPlacement, Orientation = orientation };
        var producer = new HtmlPageMatrix(uiMatrix, matrix, options, allContextInfo);
        producer.Title = "Контекстная матрица";
        var result = producer.ToHtmlString();

        File.WriteAllText(outputFile, result);
    }
}
