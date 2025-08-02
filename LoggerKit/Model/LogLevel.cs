using System.Text.Json.Serialization;

namespace LoggerKit.Model;

// context: log, model
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LogLevel
{
    None,
    Cntx,
    Err,
    Warn,
    Dbg,
    Info,
    Verb,
}