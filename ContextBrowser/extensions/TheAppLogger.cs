using System.Text.Json.Serialization;

namespace ContextBrowser.extensions;

// context: log, model
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AppLevel
{
    Roslyn,
    Puml,
    PumlTransition,
    Html,
    file
}

// context: log, model
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LogLevel
{
    None,
    Cntx,
    Err,
    Warn,
    Info,
    Dbg,
    Verb,
}
