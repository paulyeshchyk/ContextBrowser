using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Collector;
using ExporterKit;
using ExporterKit.Html;
using HtmlKit;
using HtmlKit.Page;
using HtmlKit.Page.Compiler;

namespace ExporterKit.Html.Pages.CoCompiler.DomainPerAction;

public class HtmlMatrixIndexerByNameWithClassOwnerName<TContext> : IKeyIndexBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private Dictionary<object, TContext>? _index;

    public HtmlMatrixIndexerByNameWithClassOwnerName()
    {
    }

    public Dictionary<object, TContext>? GetIndexData()
    {
        return _index;
    }

    public void Build(IEnumerable<TContext> contextsList)
    {
        if (_index == null)
        {
            _index = contextsList
                .Where(c => !string.IsNullOrWhiteSpace(c.Name))
                .GroupBy(c => c.NameWithClassOwnerName)
                .ToDictionary(
                    g => (object)g.Key,
                    g => g.First());
        }
    }
}
