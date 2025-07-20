using System.Text.RegularExpressions;

namespace ContextBrowser.Parser;

// context: csharp, regex, read
public static class CSharpParsingExpressions
{
    private static readonly string TheWordContext = "context";
    private static readonly string TheWordNamespace = "namespace";
    private static readonly string TheWordClass = "class";

    // context: regex, read
    public static Match? MatchContext(this string line) => Regex.Match(line.Trim(), @$"^//\s*{TheWordContext}:\s*(.+)", RegexOptions.IgnoreCase);

    // context: regex, read
    public static Match? MatchNamespace(this string line) => Regex.Match(line, @$"{TheWordNamespace}\s+(.+)");

    // context: regex, read
    public static Match? MatchClass(this string line) => Regex.Match(line, @$"{TheWordClass}\s+([^\s:]+)");

    // context: regex, read
    public static Match? MatchMethod(this string line) => Regex.Match(line, @"(?:public\s+)?(?:async\s+)?(?:[\w<>\?\[\]]+\s+)+(?<name>\w+)\s*\(");

    // context: regex, read
    public static Match? MatchCall(this string line) => Regex.Match(line, @"\b(\w+)\s*\(");

    // context: regex, read
    public static Match? MatchDimension(this string line) => Regex.Match(line.Trim(), @"^//\s*(\w+)\s*:\s*(.+)", RegexOptions.IgnoreCase);
}
