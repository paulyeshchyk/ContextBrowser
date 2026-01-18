using System.Text.Json.Serialization;

namespace ContextKit.Model.Classifier;

//[JsonDerivedType(typeof(EmptyDimensionClassifierDomainPerAction), "default")]
public interface IEmptyDimensionClassifier
{
    string EmptyAction { get; init; }

    string EmptyDomain { get; init; }

    bool IsEmptyAction(string actionName);

    bool IsEmptyDomain(string domainName);
}

public record EmptyDimensionClassifierDomainPerAction : IEmptyDimensionClassifier
{
    public string EmptyAction { get; init; }

    public string EmptyDomain { get; init; }

    [JsonConstructor]
    public EmptyDimensionClassifierDomainPerAction(string emptyAction, string emptyDomain)
    {
        EmptyAction = emptyAction;
        EmptyDomain = emptyDomain;
    }

    public bool IsEmptyAction(string actionName)
    {
        return string.IsNullOrWhiteSpace(actionName) || actionName.Equals(EmptyAction);
    }

    public bool IsEmptyDomain(string domainName)
    {
        return string.IsNullOrWhiteSpace(domainName) || domainName.Equals(EmptyDomain);
    }
}