using System;
using System.Collections.Generic;
using TensorKit.Model;

namespace ContextKit.Model.Classifier;

public interface ITensorClassifierDomainPerActionContext : ITensorClassifier<DomainPerActionDimensionType, ContextInfo>
{
    IEnumerable<string> MetaItems { get; }
}

// context: model, ContextInfo
// pattern: Strategy
// parsing: error
public record DomainPerActionContextTensorClassifier : ITensorClassifierDomainPerActionContext
{
    public IEnumerable<string> MetaItems { get; }

    public IContextClassifier WordRoleClassifier { get; }

    public IEmptyDimensionClassifier EmptyDimensionClassifier { get; }

    public IFakeDimensionClassifier FakeDimensionClassifier { get; }

    public DomainPerActionContextTensorClassifier(string[] metaItems, IContextClassifier wordRoleClassifier, IEmptyDimensionClassifier emptyDimensionClassifier, IFakeDimensionClassifier fakeDimensionClassifier)
    {
        MetaItems = new List<string>(metaItems);
        WordRoleClassifier = wordRoleClassifier;
        EmptyDimensionClassifier = emptyDimensionClassifier;
        FakeDimensionClassifier = fakeDimensionClassifier;
    }

    public string GetEmptyDimensionValue(int dimensionType)
    {
        return dimensionType switch
        {
            DomainPerActionDimensionType.Action => EmptyDimensionClassifier.EmptyAction,
            DomainPerActionDimensionType.Domain => EmptyDimensionClassifier.EmptyDomain,
            _ => throw new ArgumentException("Invalid dimension type.", nameof(dimensionType))
        };
    }

    public bool IsDimensionApplicable(ContextInfo ctx, string? dimensionName, int dimensionType)
    {
        if (string.IsNullOrWhiteSpace(dimensionName) || GetEmptyDimensionValue(dimensionType) == dimensionName)
        {
            return dimensionType switch
            {
                DomainPerActionDimensionType.Action => string.IsNullOrEmpty(ctx.Action),
                DomainPerActionDimensionType.Domain => ctx.Domains.Count == 0,
                _ => false
            };
        }

        return dimensionType switch
        {
            DomainPerActionDimensionType.Action => dimensionName.Equals(ctx.Action) && WordRoleClassifier.HasAllDimensionsFilled(ctx, FakeDimensionClassifier),
            DomainPerActionDimensionType.Domain => ctx.Domains.Contains(dimensionName) && WordRoleClassifier.HasAllDimensionsFilled(ctx, FakeDimensionClassifier),
            _ => false
        };
    }
}
