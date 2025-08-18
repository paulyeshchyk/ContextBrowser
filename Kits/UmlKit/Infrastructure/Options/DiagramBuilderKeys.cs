namespace UmlKit.Infrastructure.Options;

public enum DiagramBuilderKeys
{
    Transition, // "context-transition"
    MethodFlow, // "method-flow" (прежнее "MethodsOnly")
    Dependencies // "dependencies"
}

public static class DiagramBuilderKeyExtensions
{
    public static string ToKeyString(this DiagramBuilderKeys key)
    {
        return key.ToString().ToLowerInvariant();
    }
}