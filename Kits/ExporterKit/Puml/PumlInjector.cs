using ContextBrowserKit.Options.Export;

namespace ExporterKit.Puml;

public static class PumlInjector
{
    private const string SPlantUmlScript = "<render-plantuml renderMode=\"svg\">{0}</render-plantuml>";

    public record struct HtmlInjection
    {
        public string EmbeddedScript;
        public string EmbeddedContent;
    }

    // context: read, uml
    public static string ReadPumlContent(string pumlFilePath)
    {
        if(!File.Exists(pumlFilePath))
            throw new FileNotFoundException("Файл не найден", pumlFilePath);

        // Чтение всего файла в строку
        return File.ReadAllText(pumlFilePath);
    }

    public static HtmlInjection InjectActionPerDomainEmbeddedHtml(string action, string domain, ExportOptions exportOptions)
    {
        var pumlFilePath = ExportPathBuilder.BuildPath(exportOptions.Paths, ExportPathType.puml, $"composite_{action}_{domain}.puml");
        var pumlFileContent = ReadPumlContent(pumlFilePath);
        return new HtmlInjection() { EmbeddedContent = string.Format(SPlantUmlScript, pumlFileContent), EmbeddedScript = Resources.PlantUmlInjectScript };
    }

    public static HtmlInjection InjectDomainEmbeddedHtml(string domain, ExportOptions exportOptions)
    {
        var pumlFilePath = ExportPathBuilder.BuildPath(exportOptions.Paths, ExportPathType.puml, $"sequence_domain_{domain}.puml");
        var pumlFileContent = ReadPumlContent(pumlFilePath);
        return new HtmlInjection() { EmbeddedContent = string.Format(SPlantUmlScript, pumlFileContent), EmbeddedScript = Resources.PlantUmlInjectScript };
    }
}