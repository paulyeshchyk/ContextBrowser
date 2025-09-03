using ContextBrowserKit.Options.Export;
using ExporterKit.Html.Puml;
using HtmlKit.Builders.Core;
using HtmlKit.Model;
using HtmlKit.Model.Tabsheet;

namespace HtmlKit.Page.Compiler;


internal abstract class PumlTabsheetDatamodel
{
    // Абстрактный метод, который должен быть реализован в каждом дочернем классе.
    // Он отвечает только за уникальную часть — формирование имени файла.
    protected abstract string GetPumlFileName(HtmlContextInfoDataCell dto);

    public HtmlBuilder GetEmbeddedPumlInjection(HtmlContextInfoDataCell dto, ExportOptions exportOptions)
    {
        var pumlFilePath = exportOptions.FilePaths.BuildAbsolutePath(
            ExportPathType.puml,
            GetPumlFileName(dto) // Здесь используется имя файла, которое предоставляет дочерний класс
        );

        string pumlFileContent = PumlInjector.GetPumlData(exportOptions, pumlFilePath);

        return HtmlBuilderFactory.Puml(pumlFileContent, server: "http://localhost:8080");
    }
}