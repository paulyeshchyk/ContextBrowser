namespace ContextKit.Model.Classifier;

public interface IEmptyDimensionClassifier
{
    string EmptyAction { get; }

    string EmptyDomain { get; }

    bool IsEmptyAction(string actionName);

    bool IsEmptyDomain(string domainName);
}

public record EmptyDimensionClassifierDomainPerAction : IEmptyDimensionClassifier
{
    public string EmptyAction { get; }

    public string EmptyDomain { get; }

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