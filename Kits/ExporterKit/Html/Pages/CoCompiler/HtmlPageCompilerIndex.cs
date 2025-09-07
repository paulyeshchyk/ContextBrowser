using System.IO;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Collector;
using ExporterKit.Html.Matrix;
using HtmlKit.Document;
using LoggerKit;

namespace HtmlKit.Page.Compiler;

// context: html, build
public class HtmlPageCompilerIndex : IHtmlPageCompiler
{
    private readonly IAppLogger<AppLevel> _logger;

    public HtmlPageCompilerIndex(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    // context: html, build

    public void Compile(IContextInfoDataset dataset, IContextClassifier contextClassifier, ExportOptions exportOptions)
    {
        _logger.WriteLog(AppLevel.Html, LogLevel.Cntx, "--- IndexHtmlBuilder.Build ---");

        var map = ContextInfoElementTypeAndNameIndexBuilder.Build(dataset.GetAll());

        var uiMatrix = HtmlMatrixGenerator.Generate(contextClassifier, dataset, exportOptions.ExportMatrix.HtmlTable.Orientation, exportOptions.ExportMatrix.UnclassifiedPriority);
        var producer = new HtmlPageMatrix(uiMatrix, dataset, exportOptions.ExportMatrix.HtmlTable, map);
        // producer.Title = "Контекстная матрица";
        var result = producer.ToHtmlString();
        var outputFile = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.index, "index.html");

        File.WriteAllText(outputFile, result);
    }
}