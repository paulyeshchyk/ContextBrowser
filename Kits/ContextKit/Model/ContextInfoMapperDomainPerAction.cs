using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Options.Export;

namespace ContextKit.Model;

// context: ContextInfoMatrix, model
public class ContextInfoMapperDomainPerAction : IContextKeyMap<ContextInfo, IContextKey>
{
    private Dictionary<IContextKey, List<ContextInfo>>? _data;

    public Dictionary<IContextKey, List<ContextInfo>>? GetMapData()
    {
        return _data;
    }

    // context: ContextInfoMatrix, build
    public void Build(IEnumerable<ContextInfo> contextsList, ExportMatrixOptions matrixOptions, IContextClassifier contextClassifier)
    {
        _data = contextsList
                    .Where(c => !string.IsNullOrWhiteSpace(c.Name) && c.Contexts.Any())
                    .GroupBy(c =>
                    {
                        var action = c.Contexts.FirstOrDefault(contextClassifier.IsVerb);
                        var domain = c.Contexts.FirstOrDefault(contextClassifier.IsNoun);
                        return new ContextKey(
                            Action: action ?? contextClassifier.EmptyAction,
                            Domain: domain ?? contextClassifier.EmptyDomain);
                    })
                    .ToDictionary(
                        g => (IContextKey)g.Key,
                        g => g.ToList());
    }

    // context: ContextInfoMatrix, read
    public IEnumerable<string> GetDomains() => _data!.Select(k => k.Key.Domain);

    // context: ContextInfoMatrix, read
    public IEnumerable<string> GetActions() => _data!.Select(k => k.Key.Action);

    // context: ContextInfoMatrix, read
    public List<ContextInfo> GetMethodsByAction(string action) => _data!.Where(kvp => kvp.Key.Action == action).SelectMany(kvp => kvp.Value).Distinct().ToList();

    // context: ContextInfoMatrix, read
    public List<ContextInfo> GetMethodsByDomain(string domain) => _data!.Where(kvp => kvp.Key.Domain == domain).SelectMany(kvp => kvp.Value).Distinct().ToList();
}
