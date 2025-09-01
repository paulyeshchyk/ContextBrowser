using System.Collections.Generic;
using System.Text.Json;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Service;

namespace ContextBrowser.FileManager;

/// <summary>
/// Упрощенная модель данных для сериализации.
/// Сохраняет только необходимые данные и строковые идентификаторы ссылок.
/// </summary>
// context: roslyncache, model
public record ContextInfoSerializableModel
{
    public ContextInfoElementType ElementType { get; set; }

    public string Name { get; set; }

    public string FullName { get; set; }

    public string ShortName { get; set; }

    public HashSet<string> Contexts { get; set; }

    public string Namespace { get; set; }

    public string? ClassOwnerFullName { get; set; }

    public string? MethodOwnerFullName { get; set; }

    public string? Action { get; set; }

    public int SpanStart { get; set; }

    public int SpanEnd { get; set; }

    public string Identifier { get; set; }

    public HashSet<string> Domains { get; set; }

    public HashSet<string> ReferencesFullNames { get; set; }

    public HashSet<string> InvokedByFullNames { get; set; }

    public HashSet<string> PropertiesFullNames { get; set; }

    public Dictionary<string, string> Dimensions { get; set; }

    public ContextInfoSerializableModel(ContextInfoElementType elementType,
        string name,
        string fullName,
        string shortName,
        HashSet<string> contexts,
        string nameSpace,
        string? classOwnerFullName,
        string? methodOwnerFullName,
        string? action,
        int spanStart,
        int spanEnd,
        string identifier,
        HashSet<string> domains,
        HashSet<string>? referencesFullNames,
        HashSet<string>? invokedByFullNames,
        HashSet<string>? propertiesFullNames,
        Dictionary<string, string> dimensions)
    {
        ElementType = elementType;
        Name = name;
        FullName = fullName;
        ShortName = shortName;
        Contexts = contexts;
        Namespace = nameSpace;
        ClassOwnerFullName = classOwnerFullName;
        MethodOwnerFullName = methodOwnerFullName;
        Action = action;
        SpanStart = spanStart;
        SpanEnd = spanEnd;
        Identifier = identifier;
        Domains = domains;
        ReferencesFullNames = referencesFullNames ?? new();
        InvokedByFullNames = invokedByFullNames ?? new();
        PropertiesFullNames = propertiesFullNames ?? new();
        Dimensions = dimensions;
    }
}
