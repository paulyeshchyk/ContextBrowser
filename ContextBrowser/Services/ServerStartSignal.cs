namespace ContextBrowser.Services;

public interface IServerStartSignal
{
    Task WaitForSignalAsync();
    void Signal();
}

public class ServerStartSignal : IServerStartSignal
{
    private readonly TaskCompletionSource<bool> _tcs = new TaskCompletionSource<bool>();

    public Task WaitForSignalAsync()
    {
        return _tcs.Task;
    }

    public void Signal()
    {
        _tcs.TrySetResult(true);
    }
}