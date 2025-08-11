using ContextBrowserKit.Options;
using ContextKit.Model;
using ExporterKit.Matrix;
using HtmlKit.Document;
using HtmlKit.Model;

namespace ExporterKit.HtmlPageSamples;

//context: build, html
public static partial class IndexGenerator
{
    //context: build, html
    public static void GenerateContextIndexHtml(IContextClassifier contextClassifier, Dictionary<ContextContainer, List<string>> matrix, Dictionary<string, ContextInfo> allContextInfo, string outputFile, UnclassifiedPriority priority = UnclassifiedPriority.None, MatrixOrientation orientation = MatrixOrientation.DomainRows, SummaryPlacement summaryPlacement = SummaryPlacement.AfterFirst)
    {
        var uiMatrix = UiMatrixGenerator.Generate(contextClassifier, matrix, orientation, priority);
        var options = new HtmlTableOptions { SummaryPlacement = summaryPlacement, Orientation = orientation };
        var producer = new HtmlPageMatrix(uiMatrix, matrix, options, allContextInfo);
        producer.Title = "Контекстная матрица";
        var result = producer.ToHtmlString();

        File.WriteAllText(outputFile, result);
    }
}