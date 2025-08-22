using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using System.Text.Json;

namespace ContextBrowser.ContextCommentsParser;

/// <summary>
/// Управляет сохранением и чтением списка объектов ContextInfo,
/// используя промежуточную модель для обхода проблем сериализации.
/// </summary>
// context: roslyncache, build
public static class ContextListFileManager
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    /// <summary>
    /// Асинхронно сохраняет список контекстов в файл.
    /// </summary>
    // context: roslyncache, update
    public static async Task SaveContextsToCacheAsync(CacheJsonModel cacheModel, IEnumerable<ContextInfo> contextsList, CancellationToken cancellationToken)
    {
        if (File.Exists(cacheModel.Output))
        {
            try
            {
                File.Delete(cacheModel.Output);
            }
            catch (Exception ex)
            {
                throw new Exception($"Cache can't be erased\n{ex}");
            }
        }

        try
        {
            var serializableList = ContextInfoSerializableModelAdapter.Adapt(contextsList.ToList());

            var json = JsonSerializer.Serialize(serializableList, _jsonOptions);
            if (string.IsNullOrEmpty(json))
            {
                throw new Exception("Contexts list has no items");
            }

            var directoryPath = Path.GetDirectoryName(cacheModel.Output);
            if (string.IsNullOrEmpty(directoryPath))
            {
                throw new Exception($"directoryPath is empty for cache output file ({cacheModel.Output})");
            }

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            await File.WriteAllTextAsync(cacheModel.Output, json, System.Text.Encoding.UTF8, cancellationToken)
                      .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new Exception($"Cache can't be saved\n{ex}");
        }
    }

    /// <summary>
    /// Читает список контекстов из файла и восстанавливает связи.
    /// </summary>
    // context: roslyncache, read
    public static async Task<IEnumerable<ContextInfo>> ReadContextsFromCache(CacheJsonModel cacheModel, Func<CancellationToken, Task<IEnumerable<ContextInfo>>> fallback, CancellationToken cancellationToken)
    {
        if (!File.Exists(cacheModel.Input) || cacheModel.RenewCache)
        {
            return await fallback(cancellationToken).ConfigureAwait(false);
        }

        try
        {
            var fileContent = File.ReadAllText(cacheModel.Input);
            if (string.IsNullOrEmpty(fileContent))
            {
                return await fallback(cancellationToken).ConfigureAwait(false);
            }

            var serializableList = JsonSerializer.Deserialize<List<ContextInfoSerializableModel>>(fileContent, _jsonOptions);
            if (serializableList == null)
            {
                return await fallback(cancellationToken).ConfigureAwait(false);
            }

            return ContextInfoSerializableModelAdapter.ConvertToContextInfo(serializableList);
        }
        catch (Exception)
        {
            return await fallback(cancellationToken).ConfigureAwait(false);
        }
    }
}

// context: roslyncache, convert
internal static class ContextInfoSerializableModelAdapter
{
    // context: roslyncache, convert
    public static List<ContextInfo> ConvertToContextInfo(List<ContextInfoSerializableModel> serializableList)
    {
        var contexts = serializableList.Select(s => Adapt(s)).ToList();

        BuildRelations(serializableList, contexts);

        return contexts;
    }

    // context: roslyncache, convert
    public static ContextInfo Adapt(ContextInfoSerializableModel model)
    {
        return new ContextInfo(
            elementType: model.ElementType,
            identifier: model.Identifier,
            name: model.Name,
            fullName: model.FullName,
            nameSpace: model.Namespace,
            spanStart: model.SpanStart,
            spanEnd: model.SpanEnd,
            contexts: model.Contexts,
            action: model.Action,
            domains: model.Domains,
            dimensions: model.Dimensions)
        { };
    }

    // context: roslyncache, convert
    public static List<ContextInfoSerializableModel> Adapt(List<ContextInfo> contextsList)
    {
        return contextsList.Select(c => Adapt(c)).ToList();
    }

    // context: roslyncache, convert
    public static ContextInfoSerializableModel Adapt(ContextInfo contextInfo)
    {
        return new ContextInfoSerializableModel
            (
                        elementType: contextInfo.ElementType,
                               name: contextInfo.Name,
                           fullName: contextInfo.FullName,
                           contexts: contextInfo.Contexts,
                         @namespace: contextInfo.Namespace,
                 classOwnerFullName: contextInfo.ClassOwner?.FullName,
                methodOwnerFullName: contextInfo.MethodOwner?.FullName,
                             action: contextInfo.Action,
                            domains: contextInfo.Domains,
                referencesFullNames: contextInfo.References.Select(r => r.FullName).ToHashSet(),
                 invokedByFullNames: contextInfo.InvokedBy.Select(i => i.FullName).ToHashSet(),
                propertiesFullNames: contextInfo.Properties.Select(p => p.FullName).ToHashSet(),
                         dimensions: contextInfo.Dimensions,
                          spanStart: contextInfo.SpanStart,
                            spanEnd: contextInfo.SpanEnd,
                         identifier: contextInfo.Identifier);
    }

    // context: roslyncache, build
    public static void BuildRelations(List<ContextInfoSerializableModel> serializableList, List<ContextInfo> contexts)
    {
        var lookupDictionary = contexts.ToDictionary(c => c.FullName);

        // 3. Восстанавливаем связи.
        for (int i = 0; i < contexts.Count; i++)
        {
            var context = contexts[i];
            var serializableContext = serializableList[i];

            // Восстанавливаем ClassOwner и MethodOwner.
            if (serializableContext.ClassOwnerFullName != null && lookupDictionary.ContainsKey(serializableContext.ClassOwnerFullName))
            {
                context.ClassOwner = lookupDictionary[serializableContext.ClassOwnerFullName];
            }
            if (serializableContext.MethodOwnerFullName != null && lookupDictionary.ContainsKey(serializableContext.MethodOwnerFullName))
            {
                context.MethodOwner = lookupDictionary[serializableContext.MethodOwnerFullName];
            }

            // Восстанавливаем References, InvokedBy и Properties.
            foreach (var fullName in serializableContext.ReferencesFullNames)
            {
                if (lookupDictionary.TryGetValue(fullName, out var reference))
                {
                    context.References.Add(reference);
                }
            }
            foreach (var fullName in serializableContext.InvokedByFullNames)
            {
                if (lookupDictionary.TryGetValue(fullName, out var invokedBy))
                {
                    context.InvokedBy.Add(invokedBy);
                }
            }
            foreach (var fullName in serializableContext.PropertiesFullNames)
            {
                if (lookupDictionary.TryGetValue(fullName, out var property))
                {
                    context.Properties.Add(property);
                }
            }
        }
    }
}

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
        HashSet<string> contexts,
        string @namespace,
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
        Contexts = contexts;
        Namespace = @namespace;
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
