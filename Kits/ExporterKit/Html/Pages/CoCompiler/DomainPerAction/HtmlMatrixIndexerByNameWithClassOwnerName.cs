using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Collector;
using ExporterKit;
using ExporterKit.Html;
using HtmlKit;
using HtmlKit.Page;
using HtmlKit.Page.Compiler;

namespace ExporterKit.Html.Pages.CoCompiler.DomainPerAction;

public class HtmlMatrixIndexerByNameWithClassOwnerName<TContext> : DomainPerActionKeyIndexer<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private Dictionary<string, TContext>? _index;

    public Dictionary<string, TContext>? GetIndexData()
    {
        return _index;
    }

    public void Build(IEnumerable<TContext> contextsList, ExportMatrixOptions matrixOptions, IDomainPerActionContextClassifier contextClassifier)
    {
        if (_index == null)
        {
            _index = contextsList
                .Where(c => !string.IsNullOrWhiteSpace(c.Name))
                .GroupBy(c => c.NameWithClassOwnerName)
                .ToDictionary(
                    g => g.Key,
                    g => g.First());
        }
    }
}
