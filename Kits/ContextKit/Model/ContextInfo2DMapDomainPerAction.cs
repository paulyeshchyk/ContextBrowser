using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using TensorKit.Factories;
using TensorKit.Model;

namespace ContextKit.Model;

// context: ContextInfoMatrix, model
public class ContextInfo2DMapDomainPerAction : IContextInfo2DMap<ContextInfo, DomainPerActionTensor>
{
    private Dictionary<DomainPerActionTensor, List<ContextInfo>>? _data;
    private readonly ITensorFactory<DomainPerActionTensor> _keyFactory;
    private readonly ITensorBuilder _keyBuilder;
    private readonly IAppOptionsStore _optionsStore;

    public ContextInfo2DMapDomainPerAction(ITensorFactory<DomainPerActionTensor> keyFactory, ITensorBuilder keyBuilder, IAppOptionsStore optionsStore)
    {
        _keyFactory = keyFactory;
        _keyBuilder = keyBuilder;
        _optionsStore = optionsStore;
    }

    public Dictionary<DomainPerActionTensor, List<ContextInfo>>? GetMapData()
    {
        return _data;
    }

    // context: ContextInfoMatrix, build
    public Task BuildAsync(IEnumerable<ContextInfo> contextsList, CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            var classifier = _optionsStore.GetOptions<IDomainPerActionContextClassifier>();

            _data = contextsList
                        .Where(c => !string.IsNullOrWhiteSpace(c.Name) && c.Contexts.Any())
                        .GroupBy(c =>
                        {
                            var action = c.Contexts.FirstOrDefault(classifier.IsVerb) ?? classifier.EmptyAction;
                            var domain = c.Contexts.FirstOrDefault(classifier.IsNoun) ?? classifier.EmptyDomain;
                            var contextKey = _keyBuilder.BuildTensor(TensorPermutationType.Standard, new[] { action, domain }, _keyFactory.Create);
                            return contextKey;
                        })
                        .ToDictionary(
                            g => (DomainPerActionTensor)g.Key,
                            g => g.ToList());
        }, cancellationToken);
    }

    // context: ContextInfoMatrix, read
    public IEnumerable<string> GetCols() => _data!.Select(k => k.Key.Domain);

    // context: ContextInfoMatrix, read
    public IEnumerable<string> GetRows() => _data!.Select(k => k.Key.Action);

    // context: ContextInfoMatrix, read
    public List<ContextInfo> GetDataByRow(string action) => _data!.Where(kvp => kvp.Key.Action == action).SelectMany(kvp => kvp.Value).Distinct().ToList();

    // context: ContextInfoMatrix, read
    public List<ContextInfo> GetDataByCol(string domain) => _data!.Where(kvp => kvp.Key.Domain == domain).SelectMany(kvp => kvp.Value).Distinct().ToList();
}
