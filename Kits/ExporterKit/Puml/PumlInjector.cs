using System.IO;
using ContextBrowserKit.Options.Export;

namespace ExporterKit.Puml;

public static class PumlInjector
{
    private const string SPlantUmlScript = "<render-plantuml renderMode=\"svg\">{0}</render-plantuml>";

    // context: read, uml
    public static string ReadPumlContent(string pumlFilePath)
    {
        if (!File.Exists(pumlFilePath))
        {
            //throw new FileNotFoundException("Файл не найден", pumlFilePath);
            return string.Empty;
        }

        // Чтение всего файла в строку
        return File.ReadAllText(pumlFilePath);
    }

    public static PumlHtmlInjection ClassActionPerDomain(string action, string domain, ExportOptions exportOptions)
    {
        var pumlFilePath = ExportPathBuilder.BuildPath(exportOptions.Paths, ExportPathType.puml, $"class_{action}_{domain}.puml");
        var pumlFileContent = ReadPumlContent(pumlFilePath);
        return new PumlHtmlInjection() { EmbeddedContent = string.Format(SPlantUmlScript, pumlFileContent), EmbeddedScript = Resources.PlantUmlInjectScript };
    }

    public static PumlHtmlInjection InjectDomainStateEmbeddedHtml(string action, ExportOptions exportOptions)
    {
        var pumlFilePath = ExportPathBuilder.BuildPath(exportOptions.Paths, ExportPathType.puml, $"state_domain_{action}.puml");
        var pumlFileContent = ReadPumlContent(pumlFilePath);
        return new PumlHtmlInjection() { EmbeddedContent = string.Format(SPlantUmlScript, pumlFileContent), EmbeddedScript = Resources.PlantUmlInjectScript };
    }

    public static PumlHtmlInjection InjectDomainSequenceEmbeddedHtml(string domain, ExportOptions exportOptions)
    {
        var pumlFilePath = ExportPathBuilder.BuildPath(exportOptions.Paths, ExportPathType.puml, $"sequence_domain_{domain}.puml");
        var pumlFileContent = ReadPumlContent(pumlFilePath);
        return new PumlHtmlInjection() { EmbeddedContent = string.Format(SPlantUmlScript, pumlFileContent), EmbeddedScript = Resources.PlantUmlInjectScript };
    }

    public static PumlHtmlInjection InjectActionSequenceEmbeddedHtml(string action, ExportOptions exportOptions)
    {
        var pumlFilePath = ExportPathBuilder.BuildPath(exportOptions.Paths, ExportPathType.puml, $"sequence_action_{action}.puml");
        var pumlFileContent = ReadPumlContent(pumlFilePath);
        return new PumlHtmlInjection() { EmbeddedContent = string.Format(SPlantUmlScript, pumlFileContent), EmbeddedScript = Resources.PlantUmlInjectScript };
    }

    public static PumlHtmlInjection InjectActionStateEmbeddedHtml(string action, ExportOptions exportOptions)
    {
        var pumlFilePath = ExportPathBuilder.BuildPath(exportOptions.Paths, ExportPathType.puml, $"state_action_{action}.puml");
        var pumlFileContent = ReadPumlContent(pumlFilePath);
        return new PumlHtmlInjection() { EmbeddedContent = string.Format(SPlantUmlScript, pumlFileContent), EmbeddedScript = Resources.PlantUmlInjectScript };
    }
}