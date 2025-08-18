using System.Text.Json.Serialization;

namespace ContextBrowserKit.Log.Options;

// context: log, model
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LogLevel
{
    None,
    Cntx,
    Exception,
    VIP,
    Err,
    Warn,
    Dbg,
    Trace,
    Verb,
}