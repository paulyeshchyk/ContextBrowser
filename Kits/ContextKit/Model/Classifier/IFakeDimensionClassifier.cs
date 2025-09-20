namespace ContextKit.Model.Classifier;

public interface IFakeDimensionClassifier
{
    string FakeAction { get; }

    string FakeDomain { get; }
}

public record FakeDimensionClassifierDomainPerAction : IFakeDimensionClassifier
{
    public string FakeAction { get; }

    public string FakeDomain { get; }

    public FakeDimensionClassifierDomainPerAction(string fakeAction, string fakeDomain)
    {
        FakeAction = fakeAction;
        FakeDomain = fakeDomain;
    }
}
