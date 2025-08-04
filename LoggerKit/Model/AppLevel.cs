using System.Text.Json.Serialization;

namespace LoggerKit.Model;

// context: log, model
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AppLevel
{
    Roslyn,
    P_Bld,
    P_Rnd,
    P_Cpl,
    Html,
    file
}