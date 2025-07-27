using ContextBrowser.ContextKit.Model;
using ContextBrowser.ContextKit.Parser;
using ContextBrowser.DiagramFactory.Builders;
using ContextBrowser.exporter.Puml;
using ContextBrowser.HtmlKit.Builders.Core;
using ContextBrowser.HtmlKit.Page;
using ContextBrowser.UmlKit.Diagrams;
using System.Text;

namespace ContextBrowser.DiagramFactory;

// context: html, build
internal class HtmlContextDimensionBuilder
{
    private readonly Dictionary<ContextContainer, List<string>> _matrix;
    private readonly List<ContextInfo> _allContexts;
    private readonly string _outputDirectory;
    private readonly Func<string, bool> _domainCallback;
    private readonly Func<IContextDiagramBuilder> _builderFactory;

    public HtmlContextDimensionBuilder(
        Dictionary<ContextContainer, List<string>> matrix,
        List<ContextInfo> allContexts,
        string outputDirectory,
        Func<string, bool> domainCallback,
        Func<IContextDiagramBuilder> builderFactory)
    {
        _matrix = matrix;
        _allContexts = allContexts;
        _outputDirectory = outputDirectory;
        _domainCallback = domainCallback;
        _builderFactory = builderFactory;
    }

    // context: html, build
    public void Build()
    {
        GenerateDomainDiagrams();
        GenerateHtmlPages();
    }

    private void GenerateDomainDiagrams()
    {
        var classifier = new ContextClassifier();

        foreach(var domain in _matrix.Select(k => k.Key.Domain).Distinct())
        {
            if(!_domainCallback(domain))
                continue;

            var diagram = new UmlDiagramState();
            diagram.SetTitle($"Context: {domain}");
            diagram.SetSkinParam("componentStyle", "rectangle");

            var builder = _builderFactory();
            var success = builder.Build(domain, _allContexts, classifier, diagram);

            if(success)
            {
                var path = Path.Combine(_outputDirectory, $"state_domain_{domain}.puml");
                diagram.WriteToFile(path);
            }
        }
    }

    private void GenerateHtmlPages()
    {
        var allActions = _matrix.Keys.Select(k => k.Action).Distinct();
        var allDomains = _matrix.Keys.Select(k => k.Domain).Distinct();

        // --- ACTIONS ---
        foreach(var action in allActions)
        {
            var methods = _matrix
                .Where(kvp => kvp.Key.Action == action)
                .SelectMany(kvp => kvp.Value)
                .Distinct()
                .ToList();

            string embeddedScript = string.Empty;
            string embeddedContent = string.Empty;

            if(_domainCallback(action))
            {
                var puml = PumlInjector.InjectDomainEmbeddedHtml(action, _outputDirectory);
                embeddedScript = puml.EmbeddedScript;
                embeddedContent = puml.EmbeddedContent;
            }

            var path = Path.Combine(_outputDirectory, $"action_{action}.html");

            using var writer = new StreamWriter(path, false, Encoding.UTF8);

            HtmlBuilderFactory.Html.With(writer,() =>
            {
                HtmlBuilderFactory.Head.With(writer,() =>
                {
                    HtmlBuilderFactory.Meta.Cell(writer, style: "charset=\"UTF-8\"");
                    HtmlBuilderFactory.Title.Cell(writer, $"Action: {action}");
                });

                HtmlBuilderFactory.Body.With(writer,() =>
                {
                    HtmlBuilderFactory.H1.Cell(writer, $"Action: {action}");
                    HtmlBuilderFactory.Paragraph.Cell(writer, $"Methods: {methods.Count}");

                    HtmlBuilderFactory.Ul.With(writer,() =>
                    {
                        foreach(var method in methods)
                            HtmlBuilderFactory.Li.Cell(writer, method);
                    });

                    if(!string.IsNullOrWhiteSpace(embeddedContent))
                        HtmlBuilderFactory.Raw.Cell(writer, embeddedContent);
                });
            });
        }

        // --- DOMAINS ---
        foreach(var domain in allDomains)
        {
            var methods = _matrix
                .Where(kvp => kvp.Key.Domain == domain)
                .SelectMany(kvp => kvp.Value)
                .Distinct()
                .ToList();

            string embeddedScript = string.Empty;
            string embeddedContent = string.Empty;

            if(_domainCallback(domain))
            {
                var puml = PumlInjector.InjectDomainEmbeddedHtml(domain, _outputDirectory);
                embeddedScript = puml.EmbeddedScript;
                embeddedContent = puml.EmbeddedContent;
            }

            var path = Path.Combine(_outputDirectory, $"sequence_domain_{domain}.html");

            using var writer = new StreamWriter(path, false, Encoding.UTF8);

            HtmlBuilderFactory.Html.With(writer,() =>
            {
                HtmlBuilderFactory.Head.With(writer,() =>
                {
                    HtmlBuilderFactory.Meta.Cell(writer, style: "charset=\"UTF-8\"");
                    HtmlBuilderFactory.Title.Cell(writer, $"Domain: {domain}");

                    if(!string.IsNullOrWhiteSpace(embeddedScript))
                        HtmlBuilderFactory.Raw.Cell(writer, embeddedScript);
                });

                HtmlBuilderFactory.Body.With(writer,() =>
                {
                    HtmlBuilderFactory.H1.Cell(writer, $"Domain: {domain}");
                    HtmlBuilderFactory.Paragraph.Cell(writer, $"Methods: {methods.Count}");

                    HtmlBuilderFactory.Ul.With(writer,() =>
                    {
                        foreach(var method in methods)
                            HtmlBuilderFactory.Li.Cell(writer, method);
                    });

                    if(!string.IsNullOrWhiteSpace(embeddedContent))
                        HtmlBuilderFactory.Raw.Cell(writer, embeddedContent);
                });
            });
        }
    }
}
