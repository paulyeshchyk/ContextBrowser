using System;
using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Options;
using ContextKit.Model.Classifier;
using ContextKit.Model.Service;

namespace ContextKit.Model;

// context: ContextInfo, build
public class ContextInfoDataLinkGenerator
{
    private readonly ITensorClassifierDomainPerActionContext _contextClassifier;
    private readonly IWordRoleClassifier _wordRoleClassifier;
    private readonly IFakeDimensionClassifier _FakeDimensionClassifier;

    public ContextInfoDataLinkGenerator(IAppOptionsStore appOptionsStore)
    {
        _contextClassifier = appOptionsStore.GetOptions<ITensorClassifierDomainPerActionContext>();
        _wordRoleClassifier = appOptionsStore.GetOptions<IWordRoleClassifier>();
        _FakeDimensionClassifier = appOptionsStore.GetOptions<IFakeDimensionClassifier>();
    }

    // context: ContextInfo, read
    public HashSet<(string From, string To)> Generate(List<ContextInfo> elements)
    {
        var methodToCell = MapMethodsToCells(elements);
        var links = new HashSet<(string From, string To)>();

        foreach (var callerMethod in elements.Where(e => e.ElementType == ContextInfoElementType.method))
        {
            var callerId = GetMethodId(callerMethod);
            if (string.IsNullOrWhiteSpace(callerId) || !methodToCell.TryGetValue(callerId, out var fromCell))
                continue;

            var references = ContextInfoService.GetReferencesSortedByInvocation(callerMethod);
            foreach (var calledName in references)
            {
                var calleeMethod = elements.FirstOrDefault(m => string.Equals(m.Name, calledName.Name, StringComparison.OrdinalIgnoreCase));
                var calleeId = GetMethodId(calleeMethod);

                if (string.IsNullOrWhiteSpace(calleeId) || !methodToCell.TryGetValue(calleeId, out var toCell) || fromCell == toCell)
                    continue;

                links.Add((fromCell, toCell));
            }
        }

        return links;
    }

    private Dictionary<string, string> MapMethodsToCells(List<ContextInfo> elements)
    {
        var methodToCell = new Dictionary<string, string>();
        foreach (var item in elements.Where(e => e.ElementType == ContextInfoElementType.method))
        {
            var action = item.Contexts.FirstOrDefault(c => _wordRoleClassifier.IsVerb(c, _FakeDimensionClassifier));
            var domain = item.Contexts.FirstOrDefault(c => _wordRoleClassifier.IsNoun(c, _FakeDimensionClassifier));

            if (action == null || domain == null)
                continue;

            var methodId = GetMethodId(item);
            if (methodId != null)
            {
                methodToCell[methodId] = $"{action}_{domain}";
            }
        }
        return methodToCell;
    }

    private string? GetMethodId(ContextInfo? method)
    {
        if (method == null)
            return null;

        return method.ClassOwner == null
            ? method.Name
            : $"{method.ClassOwner}.{method.Name}";
    }
}
