using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using ContextBrowserKit.Commandline.Polyfills;
using TensorKit.Model;

namespace ContextKit.Model.Classifier;

public interface ITensorClassifierDomainPerActionContext<TContext> : ITensorClassifier<TContext>
    where TContext : IContextWithReferences<TContext>
{
    [CommandLineArgument("metaItems", "Контексты")]
    IEnumerable<string> MetaItems { get; init; }
}

// context: model, ContextInfo
// pattern: Strategy
// parsing: error
public record DomainPerActionContextTensorClassifier : ITensorClassifierDomainPerActionContext<ContextInfo>
{
    [CommandLineArgument("metaItems", "Контексты")]
    public IEnumerable<string> MetaItems { get; init; }

    [CommandLineArgument("wordRoleClassifier", "Контексты слов")]
    public IContextClassifier<ContextInfo> WordRoleClassifier { get; }

    [CommandLineArgument("wordRoleClassifier", "Контексты пустых измерений")]
    public IEmptyDimensionClassifier EmptyDimensionClassifier { get; }

    [CommandLineArgument("fakeDimensionClassifier", "Контексты несуществующих измерений")]
    public IFakeDimensionClassifier FakeDimensionClassifier { get; }

    [JsonConstructor]
    public DomainPerActionContextTensorClassifier(IEnumerable<string> metaItems, IContextClassifier<ContextInfo> wordRoleClassifier, IEmptyDimensionClassifier emptyDimensionClassifier, IFakeDimensionClassifier fakeDimensionClassifier)
    {
        MetaItems = metaItems;
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
