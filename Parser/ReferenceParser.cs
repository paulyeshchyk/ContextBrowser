namespace ContextBrowser.Parser;

// context: Parser
public static class ReferenceParser
{
    // context: read, csharp
    public static void EnrichWithReferences(List<ContextInfo> elements, string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        ContextInfo? currentMethod = null;
        bool insideBody = false;
        foreach (var line in lines)
        {
            var methodMatch = line.MatchMethod();
            if (methodMatch?.Success ?? false)
            {
                var name = methodMatch.Groups["name"].Value;
                currentMethod = elements.FirstOrDefault(e => e.ElementType == "method" && (e.Name?.Contains(name) ?? false));
                insideBody = true;
                continue;
            }
            if (insideBody && line.Contains("}"))
            {
                currentMethod = null;
                insideBody = false;
                continue;
            }
            if (currentMethod == null)
            {
                continue;
            }
            var callMatch = line.MatchCall();
            if (!(callMatch?.Success ?? false))
            {
                continue;
            }

            var called = callMatch.Groups[1].Value;
            if (!ReservedWords.CSharp.Contains(called) && currentMethod != null)
            {
                currentMethod.References.Add(called);
            }
        }
    }
}