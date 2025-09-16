namespace ContextBrowserKit.Options;

// context: settings, read
public interface IAppOptionsStore
{
    // context: settings, read
    void SetOptions(object options);

    // context: settings, read
    T GetOptions<T>(bool recursive = true)
        where T : class;
}
