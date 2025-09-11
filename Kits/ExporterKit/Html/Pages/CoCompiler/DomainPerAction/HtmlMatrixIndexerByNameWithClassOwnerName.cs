using System.Collections.Generic;
using System.Linq;
using ContextKit.Model;
using ContextKit.Model.Collector;
using ExporterKit;
using ExporterKit.Html;
using HtmlKit;
using HtmlKit.Page;
using HtmlKit.Page.Compiler;

namespace ExporterKit.Html.Pages.CoCompiler.DomainPerAction;

internal class HtmlMatrixIndexerByNameWithClassOwnerName<TContext> : IHtmlMatrixIndexer<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly IContextInfoDataset<TContext> _dataset;
    private Dictionary<string, TContext>? _index;

    public HtmlMatrixIndexerByNameWithClassOwnerName(IContextInfoDataset<TContext> dataset)
    {
        _dataset = dataset;
    }

    public Dictionary<string, TContext> Build()
    {
        if (_index == null)
        {
            _index = _dataset.GetAll()
                .Where(c => !string.IsNullOrWhiteSpace(c.Name))
                .GroupBy(c => c.NameWithClassOwnerName)
                .ToDictionary(
                    g => g.Key,
                    g => g.First());
        }

        return _index;
    }
}
