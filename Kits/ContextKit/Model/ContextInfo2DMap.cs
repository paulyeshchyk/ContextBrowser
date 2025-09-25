using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options;
using ContextKit.Model.Classifier;
using TensorKit.Factories;
using TensorKit.Model;

namespace ContextKit.Model;

// context: ContextInfoMatrix, model
public class ContextInfo2DMap<TTensor> : IContextInfo2DMap<ContextInfo, TTensor>
    where TTensor : IDomainPerActionTensor
{
    private Dictionary<TTensor, List<ContextInfo>>? _data;
    private readonly ITensorFactory<TTensor> _keyFactory;
    private readonly ITensorBuilder _keyBuilder;
    private readonly IAppOptionsStore _optionsStore;
    private readonly IFakeDimensionClassifier _FakeDimensionClassifier;

    public ContextInfo2DMap(ITensorFactory<TTensor> keyFactory, ITensorBuilder keyBuilder, IAppOptionsStore optionsStore)
    {
        _keyFactory = keyFactory;
        _keyBuilder = keyBuilder;
        _optionsStore = optionsStore;
        _FakeDimensionClassifier = _optionsStore.GetOptions<IFakeDimensionClassifier>();
    }

    public Dictionary<TTensor, List<ContextInfo>>? GetMapData()
    {
        return _data;
    }

    // context: ContextInfoMatrix, build
    public Task BuildAsync(IEnumerable<ContextInfo> contextsList, CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            var wordClassifier = _optionsStore.GetOptions<IWordRoleClassifier>();
            var emptyClassifier = _optionsStore.GetOptions<IEmptyDimensionClassifier>();

            _data = contextsList
                        .Where(c => !string.IsNullOrWhiteSpace(c.Name) && c.Contexts.Any())
                        .GroupBy(c =>
                        {
                            var action = c.Contexts.FirstOrDefault(c => wordClassifier.IsVerb(c, _FakeDimensionClassifier)) ?? emptyClassifier.EmptyAction;
                            var domain = c.Contexts.FirstOrDefault(c => wordClassifier.IsNoun(c, _FakeDimensionClassifier)) ?? emptyClassifier.EmptyDomain;
                            var contextKey = _keyBuilder.BuildTensor(TensorPermutationType.Standard, new[] { action, domain }, _keyFactory.Create);
                            return contextKey;
                        })
                        .ToDictionary(
                            g => (TTensor)g.Key,
                            g => g.ToList());
        }, cancellationToken);
    }

    // context: ContextInfoMatrix, read
    public IEnumerable<object> GetCols() => _data!.Select(k => k.Key.Domain);

    // context: ContextInfoMatrix, read
    public IEnumerable<object> GetRows() => _data!.Select(k => k.Key.Action);
}
