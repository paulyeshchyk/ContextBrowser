using ContextBrowser.Infrastructure;

namespace ContextBrowser.Services;

public interface IAppOptionsStore
{
    AppOptions Options { get; }
    void SetOptions(AppOptions options);
}

public class AppSettingsStore : IAppOptionsStore
{
    public AppOptions Options { get; private set; }

    public void SetOptions(AppOptions options)
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));
    }
}
