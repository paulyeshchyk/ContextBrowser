using System;

namespace UmlKit.PlantUmlSpecification.Attributes;

public static class MethodAttributesBuilder
{
    public static string? BuildUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;

        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        return $"[[[./{url}?v={timestamp}]]]";
    }
}

public static class ClassAttributesBuilder
{
    public static string? BuildUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;

        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        return $"[[./{url}?v={timestamp}]]";
    }
}
