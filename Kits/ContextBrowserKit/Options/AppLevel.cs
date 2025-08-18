using System.Text.Json.Serialization;

namespace ContextBrowserKit.Options;

// context: log, model
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AppLevel
{
    Roslyn,
    R_Parse,
    P_Tran,
    P_Bld,
    P_Rnd,
    P_Cpl,
    Html,
    file
}