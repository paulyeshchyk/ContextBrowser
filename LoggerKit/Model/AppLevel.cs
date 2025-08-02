using System.Text.Json.Serialization;

namespace LoggerKit.Model;

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