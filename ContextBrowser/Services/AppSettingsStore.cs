using ContextBrowser.Infrastructure;

namespace ContextBrowser.Services;

public interface IAppOptionsStore
{
    AppOptions Options();
    void SetOptions(AppOptions options);
}

public class AppSettingsStore : IAppOptionsStore
{
    private AppOptions? _options = null;

    public AppOptions Options() { return _options!; }

    public void SetOptions(AppOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }
}
