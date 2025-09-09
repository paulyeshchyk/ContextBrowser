using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.Html.Puml;
using HtmlKit.Builders.Core;
using HtmlKit.Model;
using HtmlKit.Model.Tabsheet;

namespace HtmlKit.Page.Compiler;

public abstract class PumlEmbeddedContentDatamodel
{
    protected abstract string GetPumlFileName(IContextKey contextKey);

    protected abstract string GetPumlFileName(string contextKey);

    public HtmlBuilder GetEmbeddedPumlInjection(IContextKey contextKey, ExportOptions exportOptions)
    {
        string fileName = GetPumlFileName(contextKey);
        return GetInjection(fileName, exportOptions);
    }

    public HtmlBuilder GetEmbeddedPumlInjection(string contextKey, ExportOptions exportOptions)
    {
        string fileName = GetPumlFileName(contextKey);
        return GetInjection(fileName, exportOptions);
    }

    private static HtmlBuilder GetInjection(string fileName, ExportOptions exportOptions)
    {
        var pumlFilePath = exportOptions.FilePaths.BuildAbsolutePath(
            ExportPathType.puml,
            fileName);

        string pumlFileContent = PumlInjector.GetPumlData(exportOptions, pumlFilePath);

        return HtmlBuilderFactory.Puml(pumlFileContent, server: "http://localhost:8080");
    }
}