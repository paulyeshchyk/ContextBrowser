using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using TensorKit.Factories;
using TensorKit.Model;

namespace ContextKit.Model;

// context: ContextInfoMatrix, model
public class ContextInfoMapperDomainPerAction : DomainPerActionKeyMap<ContextInfo, DomainPerActionTensor>
{
    private Dictionary<DomainPerActionTensor, List<ContextInfo>>? _data;
    private readonly ITensorFactory<DomainPerActionTensor> _keyFactory;
    private readonly ITensorBuilder _keyBuilder;

    public ContextInfoMapperDomainPerAction(ITensorFactory<DomainPerActionTensor> keyFactory, ITensorBuilder keyBuilder)
    {
        _keyFactory = keyFactory;
        _keyBuilder = keyBuilder;
    }

    public Dictionary<DomainPerActionTensor, List<ContextInfo>>? GetMapData()
    {
        return _data;
    }

    // context: ContextInfoMatrix, build
    public void Build(IEnumerable<ContextInfo> contextsList, ExportMatrixOptions matrixOptions, IDomainPerActionContextClassifier contextClassifier)
    {
        _data = contextsList
                    .Where(c => !string.IsNullOrWhiteSpace(c.Name) && c.Contexts.Any())
                    .GroupBy(c =>
                    {
                        var action = c.Contexts.FirstOrDefault(contextClassifier.IsVerb) ?? contextClassifier.EmptyAction;
                        var domain = c.Contexts.FirstOrDefault(contextClassifier.IsNoun) ?? contextClassifier.EmptyDomain;
                        var contextKey = _keyBuilder.BuildTensor(TensorPermutationType.Standard, new[] { action, domain }, _keyFactory.Create);
                        return contextKey;
                    })
                    .ToDictionary(
                        g => (DomainPerActionTensor)g.Key,
                        g => g.ToList());
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
