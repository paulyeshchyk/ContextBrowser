using ContextBrowser.Infrastructure;
using ContextKit.Model;

namespace ContextBrowser.Services;

public class ContextClassifierBuilder : IContextClassifierBuilder
{

    private readonly IAppOptionsStore _optionsStore;

    public ContextClassifierBuilder(IAppOptionsStore optionsStore)
    {
        _optionsStore = optionsStore;
    }

    public IContextClassifier Build()
    {
        return _optionsStore.Options().Classifier;
    }

}