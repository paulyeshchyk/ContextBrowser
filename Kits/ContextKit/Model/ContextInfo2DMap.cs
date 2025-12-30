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

    // context: ContextInfoMatrix, read
    public IEnumerable<ILabeledValue> GetCols() => _data!.Select(k => new StringColumnWrapper(k.Key.Domain, k.Key.Domain));

    // context: ContextInfoMatrix, read
    public IEnumerable<ILabeledValue> GetRows() => _data!.Select(k => new StringColumnWrapper(k.Key.Action, k.Key.Action));

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

    private record ContextPair(ContextInfo Context, string Action, string Domain);

    private IEnumerable<ContextPair> GetContextPairs(
        ContextInfo context,
        IContextClassifier wordClassifier,
        IEmptyDimensionClassifier emptyClassifier)
    {
        // Извлекаем и подготавливаем действия
        var actions = context.Contexts.Where(ctx => wordClassifier.IsVerb(ctx, _FakeDimensionClassifier)).ToList();
        var effectiveActions = actions.Any() ? actions : new List<string> { emptyClassifier.EmptyAction };

        // Извлекаем и подготавливаем домены
        var domains = context.Contexts.Where(ctx => wordClassifier.IsNoun(ctx, _FakeDimensionClassifier)).ToList();
        var effectiveDomains = domains.Any() ? domains : new List<string> { emptyClassifier.EmptyDomain };

        // Декартово произведение (Action x Domain)
        return effectiveActions.SelectMany(action =>
            effectiveDomains.Select(domain =>
                new ContextPair(context, action, domain)));
    }

    // context: ContextInfoMatrix, build
    public Task BuildAsync(IEnumerable<ContextInfo> contextsList, CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            var wordClassifier = _optionsStore.GetOptions<IContextClassifier>();
            var emptyClassifier = _optionsStore.GetOptions<IEmptyDimensionClassifier>();

            _data = contextsList
                .Where(c => !string.IsNullOrWhiteSpace(c.Name) && c.Contexts.Any())

                // 1. Разворачивание
                .SelectMany(c => GetContextPairs(c, wordClassifier, emptyClassifier))

                // 2. Группировка
                .GroupBy(pair =>
                {
                    // Формирование TTensor ключа
                    var contextKey = _keyBuilder.BuildTensor(
                        TensorPermutationType.Standard,
                        new object[] { pair.Action, pair.Domain },
                        _keyFactory.Create);
                    return contextKey;
                })

                // 3. Агрегация
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(pair => pair.Context).ToList());
        }, cancellationToken);
    }
}
