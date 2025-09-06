using System;

namespace UmlKit.PlantUmlSpecification.Attributes;

public static class MethodAttributesBuilder
{
    // только тройная скоба
    public static string? BuildUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;
        return $"[[[./{url}]]]";
    }
}

public static class ClassAttributesBuilder
{
    // только двойная скоба
    public static string? BuildUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;
        return $"[[./{url}]]";
    }
}
