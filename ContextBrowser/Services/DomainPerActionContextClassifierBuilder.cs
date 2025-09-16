using ContextBrowser.Infrastructure;
using ContextKit.Model;

namespace ContextBrowser.Services;

public class DomainPerActionContextClassifierBuilder : IDomainPerActionContextClassifierBuilder
{
    private readonly IAppOptionsStore _optionsStore;

    public DomainPerActionContextClassifierBuilder(IAppOptionsStore optionsStore)
    {
        _optionsStore = optionsStore;
    }

    public IDomainPerActionContextClassifier Build()
    {
        return _optionsStore.Options().Classifier;
    }
}