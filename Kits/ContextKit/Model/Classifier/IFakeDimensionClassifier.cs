using System.Text.Json.Serialization;

namespace ContextKit.Model.Classifier;

//[JsonDerivedType(typeof(FakeDimensionClassifierDomainPerAction), "default")]
public interface IFakeDimensionClassifier
{
    string FakeAction { get; init; }

    string FakeDomain { get; init; }
}

public record FakeDimensionClassifierDomainPerAction : IFakeDimensionClassifier
{
    public string FakeAction { get; init; }

    public string FakeDomain { get; init; }

    public FakeDimensionClassifierDomainPerAction(string fakeAction, string fakeDomain)
    {
        FakeAction = fakeAction;
        FakeDomain = fakeDomain;
    }
}
