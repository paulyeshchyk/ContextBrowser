using ContextBrowserKit.Options.Export;

namespace ExporterKit.Puml;

public record struct PumlHtmlInjection
{
    public string EmbeddedScript;
    public string EmbeddedContent;
}

public delegate PumlHtmlInjection PumlHtmlInjectionGeneratorActionAndDomain(string action, string domain, ExportOptions options);
public delegate PumlHtmlInjection PumlHtmlInjectionGeneratorAction(string action, ExportOptions options);
public delegate PumlHtmlInjection PumlHtmlInjectionGeneratorDomain(string domain, ExportOptions options);
