using System;
using System.Collections.Generic;
using System.Linq;
using ContextKit.Model;
using ContextKit.Model.Service;

namespace ContextBrowser.DiagramFactory.Exporters;

// context: ContextInfo, build
public class ContextInfoDataLinkGenerator
{
    private readonly IDomainPerActionContextClassifier _contextClassifier;
    private readonly List<ContextInfo> _elements;

    public ContextInfoDataLinkGenerator(IDomainPerActionContextClassifier contextClassifier, List<ContextInfo> elements)
    {
        _contextClassifier = contextClassifier;
        _elements = elements;
    }

    // context: ContextInfo, read
    public HashSet<(string From, string To)> Generate()
    {
        var methodToCell = MapMethodsToCells();
        var links = new HashSet<(string From, string To)>();

        foreach (var callerMethod in _elements.Where(e => e.ElementType == ContextInfoElementType.method))
        {
            var callerId = GetMethodId(callerMethod);
            if (string.IsNullOrWhiteSpace(callerId) || !methodToCell.TryGetValue(callerId, out var fromCell))
                continue;

            var references = ContextInfoService.GetReferencesSortedByInvocation(callerMethod);
            foreach (var calledName in references)
            {
                var calleeMethod = _elements.FirstOrDefault(m => string.Equals(m.Name, calledName.Name, StringComparison.OrdinalIgnoreCase));
                var calleeId = GetMethodId(calleeMethod);

                if (string.IsNullOrWhiteSpace(calleeId) || !methodToCell.TryGetValue(calleeId, out var toCell) || fromCell == toCell)
                    continue;

                links.Add((fromCell, toCell));
            }
        }

        return links;
    }

    private Dictionary<string, string> MapMethodsToCells()
    {
        var methodToCell = new Dictionary<string, string>();
        foreach (var item in _elements.Where(e => e.ElementType == ContextInfoElementType.method))
        {
            var action = item.Contexts.FirstOrDefault(_contextClassifier.IsVerb);
            var domain = item.Contexts.FirstOrDefault(_contextClassifier.IsNoun);

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
