using Microsoft.CodeAnalysis;
using System.Text.RegularExpressions;

namespace ContextKit.Model;

// coverage: 255
// context: ContextInfo, model
public class ContextInfo : IContextWithReferences<ContextInfo>
{
    private static readonly Regex InvalidCharsRegex = new("[^a-zA-Z0-9_]", RegexOptions.Compiled);

    public ContextInfoElementType ElementType { get; set; } = ContextInfoElementType.none;

    public string? Name { get; set; }

    public string SymbolName { get; set; } = string.Empty;

    public HashSet<string> Contexts { get; set; } = new();

    public string? Namespace { get; set; }

    public ContextInfo? ClassOwner { get; set; }

    public ContextInfo? MethodOwner { get; set; }

    public string? Action { get; set; }

    public HashSet<string> Domains { get; } = new();

    public HashSet<ContextInfo> References { get; set; } = new();

    public HashSet<ContextInfo> InvokedBy { get; set; } = new();

    public Dictionary<string, string> Dimensions { get; set; } = new();

    public string PlantUmlId => InvalidCharsRegex.Replace(SymbolName, "_");

    public SyntaxNode? SyntaxNode { get; set; }

    public override int GetHashCode() => SymbolName.GetHashCode();

    public override bool Equals(object? obj) => obj is ContextInfo other && SymbolName == other.SymbolName;
}