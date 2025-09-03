using System.IO;
using ContextBrowser.Infrastructure;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.Html;
using ExporterKit.Html.Matrix;
using HtmlKit.Document;
using LoggerKit;

namespace HtmlKit.Page.Compiler;

// context: html, build
public class HtmlIndexBuilder : IHtmlPageCompiler
{
    private readonly IAppLogger<AppLevel> _logger;

    public HtmlIndexBuilder(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    // context: html, build

    public void Compile(IContextInfoDataset model, IContextClassifier contextClassifier, ExportOptions exportOptions)
    {
        _logger.WriteLog(AppLevel.Html, LogLevel.Cntx, "--- IndexHtmlBuilder.Build ---");

        var uiMatrix = HtmlMatrixGenerator.Generate(contextClassifier, model.ContextInfoData, exportOptions.ExportMatrix.HtmlTable.Orientation, exportOptions.ExportMatrix.UnclassifiedPriority);
        var producer = new HtmlPageMatrix(uiMatrix, model.ContextInfoData, exportOptions.ExportMatrix.HtmlTable, model.ContextLookup);
        producer.Title = "Контекстная матрица";
        var result = producer.ToHtmlString();
        var outputFile = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.index, "index.html");
        File.WriteAllText(outputFile, result);
    }
}