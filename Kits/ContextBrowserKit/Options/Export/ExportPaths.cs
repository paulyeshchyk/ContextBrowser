using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ContextBrowserKit.Options.Export;

// context: settings, model
public abstract class ExportPaths
{
    public string OutputDirectory { get; set; } = string.Empty;

    public CacheJsonModel CacheModel { get; set; } = new CacheJsonModel();

    public IReadOnlyDictionary<ExportPathType, string> RelativePaths { get; set; } = new Dictionary<ExportPathType, string>();

    public ExportPaths()
    {
    }

    public ExportPaths(string outputDirectory, CacheJsonModel cacheModel, IReadOnlyDictionary<ExportPathType, string> paths)
    {
        OutputDirectory = outputDirectory;
        CacheModel = cacheModel;
        RelativePaths = paths;
    }

    // context: settings, file, update
    public abstract string BuildAbsolutePath(string from, params string[] files);

    // context: settings, file, update
    public abstract string BuildRelativePath(string from, params string[] files);

    // context: settings, file, update
    public string BuildRelativePath(ExportPathType pathType, params string[] files)
    {
        var p = GetRelativePath(pathType);
        var result = BuildRelativePath(p, files);
        return result;
    }

    // context: settings, file, update
    public string BuildAbsolutePath(ExportPathType pathType, params string[] files)
    {
        var p = GetRelativePath(pathType);
        var result = BuildAbsolutePath(p, files);
        return result;
    }

    // context: settings, file, update
    public string GetRelativePath(ExportPathType type)
    {
        if (RelativePaths.TryGetValue(type, out var path))
            return path;
        throw new InvalidOperationException($"Export path not found for {type}");
    }

    // context: settings, file, update
    public ExportPathType[] GetPathTypes()
    {
        return RelativePaths.Keys.ToArray();
    }

    // context: settings, file, update
    public string[] GetPaths()
    {
        return RelativePaths.Values.ToArray();
    }

    // context: settings, file, update
    internal static Dictionary<ExportPathType, string> ConvertToDictionary(ExportPathItem[] paths)
    {
        var d = new Dictionary<ExportPathType, string>();
        foreach (var pathItem in paths)
        {
            d[pathItem.Type] = pathItem.Path;
        }

        return d;
    }
}