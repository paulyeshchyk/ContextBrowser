using System.Text.RegularExpressions;

namespace ContextBrowser.Parser;

public static class ParsingExpressions
{
    public static Match? MatchNamespace(this string line) => Regex.Match(line, @"namespace\s+(.+)");

    public static Match? MatchContext(this string line) => Regex.Match(line.Trim(), @"^//\s*context:\s*(.+)", RegexOptions.IgnoreCase);

    public static Match? MatchClass(this string line) => Regex.Match(line, @"class\s+([^\s:]+)");

    public static Match? MatchMethod(this string line) => Regex.Match(line, @"(?:public\s+)?(?:async\s+)?(?:[\w<>\?\[\]]+\s+)+(?<name>\w+)\s*\(");

    public static Match? MatchCall(this string line) => Regex.Match(line, @"\b(\w+)\s*\(");

    internal static string Escaped(this string str)
    {
        return str.Replace("<", "&lt;")
                  .Replace(">", "&gt;");
    }
}
