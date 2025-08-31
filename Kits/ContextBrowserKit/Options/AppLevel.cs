using System.Text.Json.Serialization;

namespace ContextBrowserKit.Options;

// context: log, model
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AppLevel
{
    App,
    R_Symbol,
    R_Syntax,
    R_Dll,
    R_Cntx,
    R_Invocation,
    R_Comments,
    P_Tran,
    P_Bld,
    P_Rnd,
    P_Cpl,
    Html,
    file
}