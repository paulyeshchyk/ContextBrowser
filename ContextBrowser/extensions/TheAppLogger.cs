using System.Text.Json.Serialization;

namespace ContextBrowser.extensions;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AppLevel
{
    Roslyn,
    Puml,
    PumlTransition,
    Html,
    file
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LogLevel
{
    None,
    Cntx,
    Err,
    Warn,
    Info,
    Verb,
    Dbg,
}
