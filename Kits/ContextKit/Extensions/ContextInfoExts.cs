using ContextKit.Model;

namespace ContextKit.Extensions;

public static class ContextInfoExts
{
    public static string GetDebugName(this ContextInfo contextInfo)
    {
        if (contextInfo.Symbol != null)
        {
            return contextInfo.Symbol.ToDisplayString();
        }
        return $"{(contextInfo.MethodOwner?.Name ?? string.Empty)}.{contextInfo.Name}";
    }

    public static string GetDebugSymbolName(this ContextInfo contextInfo)
    {
        var shortest = contextInfo.Symbol?.GetShortestName();
        if (string.IsNullOrWhiteSpace(shortest))
            return contextInfo.Name;
        var classownerName = contextInfo.ClassOwner?.Name;
        if (string.IsNullOrWhiteSpace(classownerName))
            return contextInfo.Name;

        return $"{classownerName}.{shortest}";
    }
}