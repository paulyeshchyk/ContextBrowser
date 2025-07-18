using System.Text.RegularExpressions;

namespace ContextBrowser.Parser;

// context: csharp, model
public static class CSharpParsingExpressions
{
    private static readonly string TheWordContext = "context";
    private static readonly string TheWordNamespace = "namespace";
    private static readonly string TheWordClass = "class";

    public static Match? MatchContext(this string line) => Regex.Match(line.Trim(), @$"^//\s*{TheWordContext}:\s*(.+)", RegexOptions.IgnoreCase);

    public static Match? MatchNamespace(this string line) => Regex.Match(line, @$"{TheWordNamespace}\s+(.+)");

    public static Match? MatchClass(this string line) => Regex.Match(line, @$"{TheWordClass}\s+([^\s:]+)");

    public static Match? MatchMethod(this string line) => Regex.Match(line, @"(?:public\s+)?(?:async\s+)?(?:[\w<>\?\[\]]+\s+)+(?<name>\w+)\s*\(");

    public static Match? MatchCall(this string line) => Regex.Match(line, @"\b(\w+)\s*\(");
}
