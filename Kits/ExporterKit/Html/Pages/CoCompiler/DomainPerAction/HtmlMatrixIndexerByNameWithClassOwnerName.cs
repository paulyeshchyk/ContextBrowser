using System.Collections.Generic;
using System.Linq;
using ContextKit.Model;

namespace ExporterKit.Html.Pages.CoCompiler.DomainPerAction;

public class HtmlMatrixIndexerByNameWithClassOwnerName<TContext, TTensor> : IKeyIndexBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
    where TTensor : notnull
{
    private Dictionary<object, TContext>? _index;
    private readonly IContextInfoDatasetProvider<TTensor> _datasetProvider;

    public HtmlMatrixIndexerByNameWithClassOwnerName(IContextInfoDatasetProvider<TTensor> datasetProvider)
    {
        _datasetProvider = datasetProvider;
    }

    public Dictionary<object, TContext>? Build(IEnumerable<TContext> contextsList)
    {
        if (_index == null)
        {
            _index = contextsList?
                .Where(c => !string.IsNullOrWhiteSpace(c.Name))
                .GroupBy(c => c.NameWithClassOwnerName)
                .ToDictionary(
                    g => (object)g.Key,
                    g => g.First());
        }

        return _index;
    }
}
