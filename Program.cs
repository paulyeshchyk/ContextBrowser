using ContextBrowser.ContextKit.graph;
using ContextBrowser.ContextKit.Matrix;
using ContextBrowser.ContextKit.Model;
using ContextBrowser.ContextKit.Parser;
using ContextBrowser.exporter;
using ContextBrowser.exporter.HtmlPageSamples;
using ContextBrowser.Extensions;
using ContextBrowser.HtmlKit.Exporter;
using ContextBrowser.HtmlKit.Model;
using ContextBrowser.UmlKit.Diagrams;
using ContextBrowser.UmlKit.Exporter;

namespace ContextBrowser.ContextCommentsParser;

static class Program
{
    public static void Main(string[] args)
    {
        string theSourcePath = ".\\..\\..\\..\\..\\ContextBrowser";
        string theOutputPath = ".\\output\\";

        bool includeAllStandardActions = true;
        var summaryPlacement = SummaryPlacement.AfterLast;
        var matrixOrientation = MatrixOrientation.DomainRows;
        var unclassifiedPriority = UnclassifiedPriority.Highest;

        var contextsList = ContextParser.Parse(theSourcePath);
        var contextClassifier = new ContextClassifier();
        var matrix = ContextMatrixUmlExporter.GenerateMatrix(contextsList, contextClassifier, unclassifiedPriority, includeAllStandardActions);

        var contextLookup = contextsList
            .Where(c => !string.IsNullOrWhiteSpace(c.Name))
            .GroupBy(c => c.ElementType == ContextInfoElementType.method && !string.IsNullOrWhiteSpace(c.ClassOwner?.Name)
                ? $"{c.ClassOwner?.Name}.{c.Name}"
                : $"{c.Name}")
            .ToDictionary(g => g.Key, g => g.First());

        FileUtils.WipeDirectory(theOutputPath);
        FileUtils.CreateDirectoryIfNotExists(theOutputPath);


        //ContextMatrixUmlExporter.GenerateCsv(matrix, $"{theOutputPath}matrix.csv");
        //HeatmapExporter.GenerateHeatmapCsv(matrix, $"{theOutputPath}heatmap.csv", unclassifiedPriority);


        UmlContextPackagesDiagram.Build(contextsList, $"{theOutputPath}uml.packages.domains.puml");
        UmlContextMethodsOnlyDiagram.Build(contextsList, $"{theOutputPath}methodlinks.puml");
        UmlContextMethodPerActionDomainDiagram.Build(matrix, $"{theOutputPath}uml.packages.actions.puml");

        var links = ContextMatrixUmlExporter.GenerateMethodLinks(contextsList, contextClassifier);
        UmlContextRelationDiagram.GenerateLinksUml(links, $"{theOutputPath}uml.4.links.puml");


        // 1.
        UmlContextComponentDiagram.Build(matrix, theOutputPath);

        // 2.
        UmlContextActionPerDomainDiagram.Build(matrix,(action, domain) => $"composite_{action}_{domain}.puml", $"{theOutputPath}uml.heatmap.link.puml");

        // 3.
        HtmlIndexPage.GenerateContextHtmlPages(matrix, theOutputPath);

        // 4.
        IndexGenerator.GenerateContextIndexHtml(
            matrix,
            contextLookup,
            outputFile: $"{theOutputPath}index.html",
            priority: unclassifiedPriority,
            orientation: matrixOrientation,
            summaryPlacement: summaryPlacement
            );

        HtmlDimensionPage.GenerateContextDimensionHtmlPages(matrix, theOutputPath,(domain) => SingleDomainPass(domain, contextsList, contextClassifier));

        SingleItemPass("ContextMatrixUmlExporter", contextsList, contextClassifier);
    }

    private static bool SingleDomainPass(string domainName, List<ContextInfo> contextsList, ContextClassifier contextClassifier)
    {
        var filteredForDomain = contextsList.Where(s => s.Domains.Any(s => string.Equals(s, domainName, StringComparison.OrdinalIgnoreCase)));
        if(!filteredForDomain.Any())
        {
            Console.WriteLine($"domain ({domainName}) not found");
            return false;
        }

        var ud = new UmlDiagramSequence();
        ud.SetTitle($"Domain: {domainName}");
        ud.SetSkinParam("componentStyle", "rectangle");

        var itemWalker = new ItemWalker(
            onGetDescendants:(s) => contextsList.Where(d => d.ClassOwner == s),
            onGetDomainItems:(d) => contextsList.Where(c => c.Contexts.Contains(d) && contextClassifier.IsNoun(d)),
            onExportItem:(item, descendant, descendantDomain, domain) =>
            {
                bool skipSelf = false;
                if((item == descendantDomain) && skipSelf)
                {
                    return;
                }

                var subjectName = descendantDomain?.Name ?? "u1";

                var linkName = $"{descendant?.Name ?? "UNKNOWN"} [{subjectName.Replace(".", "_")}]";
                var iname = item.Name ?? "<empty2>";

                ud.AddTransition(domain, iname, linkName);
            },
            visitCallback:(s) => Console.WriteLine($"[VISITED] {s.Name}"));

        itemWalker.Walk(filteredForDomain);
        ud.WriteToFile($".\\output\\links_domain_{domainName}.puml");
        return true;
    }

    private static bool SingleItemPass(string itemClassName, List<ContextInfo> contextsList, ContextClassifier contextClassifier)
    {
        var ud = new UmlDiagramSequence();
        ud.SetTitle($"Class {itemClassName}");
        ud.SetSkinParam("componentStyle", "rectangle");

        var itemWalker = new ItemWalker(
            onGetDescendants:(s) => contextsList.Where(d => d.ClassOwner == s),
            onGetDomainItems:(d) => contextsList.Where(c => c.Contexts.Contains(d) && contextClassifier.IsNoun(d)),
            onExportItem:(item, descendant, descendantDomain, domain) =>
            {
                bool skipSelf = true;
                if((item == descendantDomain) && skipSelf)
                {
                    return;
                }

                var theContext = string.IsNullOrWhiteSpace(domain) ? "unknown context" : domain;
                var theOName = descendantDomain?.Name ?? "u1";

                var linkName = $"{descendant?.Name ?? "UNKNOWN"} <<{theOName}>>";
                var iname = item.Name ?? "<empty2>";

                ud.AddTransition(iname, domain, linkName);
            },
            visitCallback:(s) => Console.WriteLine($"[VISITED] {s.Name}"));

        var owner = contextsList.Where(s => s.Name?.Equals(itemClassName) ?? false).FirstOrDefault();
        if(owner == null)
        {
            Console.WriteLine("item not found");
            return false;
        }
        itemWalker.Walk(owner);
        ud.WriteToFile($".\\output\\links_{itemClassName}.puml");
        return true;
    }
}
