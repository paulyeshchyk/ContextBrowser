using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;

namespace UmlKit.Builders.Model;

// parsing: error
public class RenderContextActivationStack : Stack<string>
{
    private readonly OnWriteLog? _onWriteLog;

    public RenderContextActivationStack(OnWriteLog? onWriteLog) : base()
    {
        _onWriteLog = onWriteLog;
    }

    public void TryPush(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Trace, "[PUSH_FAIL]: item is empty");
            return;
        }

        _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Trace, $"[PUSH_OK]: {value}");
        Push(value);
    }

    public bool TryPeekThenLog(out string? result)
    {
        var isSuccess = TryPeek(out result);
        if (isSuccess)
        {
            _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Trace, $"[PEEK_OK]: {result}");
        }
        else
        {
            _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Err, $"[PEEK_FAIL] Can't peek");
        }
        return isSuccess;
    }
}