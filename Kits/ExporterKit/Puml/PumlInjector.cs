using ContextBrowserKit.Options.Export;

namespace ExporterKit.Puml;

public static class PumlInjector
{
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
        var pumlEmbeddedScript = "<script type=\"module\">import enableElement from \"https://cdn.pika.dev/render-plantuml\";enableElement();</script>";
        return new HtmlInjection() { EmbeddedContent = $"<render-plantuml renderMode=\"svg\">{pumlFileContent}</render-plantuml>", EmbeddedScript = pumlEmbeddedScript };
    }

    public static HtmlInjection InjectDomainEmbeddedHtml(string domain, ExportOptions exportOptions)
    {
        var pumlFilePath = ExportPathBuilder.BuildPath(exportOptions.Paths, ExportPathType.puml, $"sequence_domain_{domain}.puml");
        var pumlFileContent = ReadPumlContent(pumlFilePath);
        var pumlEmbeddedScript = "<script type=\"module\">import enableElement from \"https://cdn.pika.dev/render-plantuml\";enableElement();</script>";
        return new HtmlInjection() { EmbeddedContent = $"<render-plantuml renderMode=\"svg\">{pumlFileContent}</render-plantuml>", EmbeddedScript = pumlEmbeddedScript };
    }
}