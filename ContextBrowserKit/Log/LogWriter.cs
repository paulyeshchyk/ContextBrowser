using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;

namespace ContextBrowserKit.Log;

// context: log, share
public delegate void LogWriter(string message);

// context: log, share
public delegate void OnWriteLog(AppLevel al, LogLevel ll, string message, LogLevelNode levelNode = LogLevelNode.None);

