using ContextKit.Matrix;
using ContextKit.Model;
using HtmlKit.Document;
using HtmlKit.Model;

namespace HtmlKit.Exporter;

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