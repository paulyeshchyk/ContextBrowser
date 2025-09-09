using System;
using ContextBrowser.Infrastructure;

namespace ContextBrowser.Services;

// context: settings, read
public interface IAppOptionsStore
{
    // context: settings, read
    AppOptions Options();

    // context: settings, update
    void SetOptions(AppOptions options);
}

// context: settings, read
public class AppSettingsStore : IAppOptionsStore
{
    private AppOptions? _options = null;

    // context: settings, read
    public AppOptions Options() { return _options!; }

    // context: settings, update
    public void SetOptions(AppOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }
}
