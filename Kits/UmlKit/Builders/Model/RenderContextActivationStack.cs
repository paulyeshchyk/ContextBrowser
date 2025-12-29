using System.Collections.Generic;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using LoggerKit;

namespace UmlKit.Builders.Model;

// parsing: error
public class RenderContextActivationStack : Stack<string>
{
    private readonly IAppLogger<AppLevel> _logger;

    public RenderContextActivationStack(IAppLogger<AppLevel> logger) : base()
    {
        _logger = logger;
    }

    public void TryPush(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            _logger.WriteLog(AppLevel.P_Rnd, LogLevel.Trace, "[PUSH_FAIL]: item is empty");
            return;
        }

        _logger.WriteLog(AppLevel.P_Rnd, LogLevel.Trace, $"[PUSH_OK]: {value}");
        Push(value);
    }

    public bool TryPeekThenLog(out string? result)
    {
        var isSuccess = TryPeek(out result);
        if (isSuccess)
        {
            _logger.WriteLog(AppLevel.P_Rnd, LogLevel.Trace, $"[PEEK_OK]: {result}");
        }
        else
        {
            _logger.WriteLog(AppLevel.P_Rnd, LogLevel.Err, $"[PEEK_FAIL] Can't peek");
        }
        return isSuccess;
    }
}