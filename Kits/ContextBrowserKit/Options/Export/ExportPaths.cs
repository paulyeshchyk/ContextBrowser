using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ContextBrowserKit.Options.Export;

// context: file, update
public sealed class ExportFilePaths : ExportPaths
{
    public ExportFilePaths(string outputDirectory, CacheJsonModel cacheModel, IReadOnlyDictionary<ExportPathType, string> paths) : base(outputDirectory, cacheModel, paths)
    {
    }

    public ExportFilePaths(string outputDirectory, CacheJsonModel cacheModel, params ExportPathItem[] paths) : base(outputDirectory, cacheModel, ConvertToDictionary(paths))
    {
    }

    public override string BuildAbsolutePath(string from, params string[] files)
    {
        string filesPath = Path.Combine(files);

        string combinedPath = OutputDirectory.Equals(from)
            ? Path.Combine(from, filesPath)
            : Path.Combine(OutputDirectory, from, filesPath);

        return Path.GetFullPath(combinedPath);
    }
}

// context: file, update
public sealed class ExportWebPaths : ExportPaths
{
    public ExportWebPaths(string outputDirectory, CacheJsonModel cacheModel, IReadOnlyDictionary<ExportPathType, string> paths) : base(outputDirectory, cacheModel, paths)
    {
    }

    public ExportWebPaths(string outputDirectory, CacheJsonModel cacheModel, params ExportPathItem[] paths) : base(outputDirectory, cacheModel, ConvertToDictionary(paths))
    {
    }

    // context: file, update
    public override string BuildAbsolutePath(string from, params string[] files)
    {
        string relativePathFromFiles = string.Join("/", files);

        string baseUrl;
        if (from.Equals(OutputDirectory))
        {
            baseUrl = OutputDirectory.TrimEnd('/');
        }
        else
        {
            string cleanedFrom = from.Replace("\\", "/").Replace("//", "/").TrimStart('.', '/');
            baseUrl = $"{OutputDirectory.TrimEnd('/')}/{cleanedFrom}";
        }

        string cleanedRelativePath = relativePathFromFiles.Replace("\\", "/").Replace("//", "/").TrimStart('.', '/');

        return $"{baseUrl}/{cleanedRelativePath}";
    }
}

// context: settings, model
public abstract class ExportPaths
{
    public string OutputDirectory { get; set; }

    public CacheJsonModel CacheModel { get; set; }

    public IReadOnlyDictionary<ExportPathType, string> RelativePaths { get; set; }

    public ExportPaths(string outputDirectory, CacheJsonModel cacheModel, IReadOnlyDictionary<ExportPathType, string> paths)
    {
        OutputDirectory = outputDirectory;
        CacheModel = cacheModel;
        RelativePaths = paths;
    }

    // context: settings, file, update
    public abstract string BuildAbsolutePath(string from, params string[] files);

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

// context: roslyncache, model
public class CacheJsonModel
{
    public string Output { get; set; }

    public string Input { get; set; }

    public bool RenewCache { get; set; }

    public CacheJsonModel(string? output, string? input, bool renewCache)
    {
        Output = output ?? string.Empty;
        Input = input ?? string.Empty;
        RenewCache = renewCache;
    }
}

// context: settings, model
public class ExportPathItem
{
    public ExportPathType Type { get; }

    public string Path { get; }

    public ExportPathItem(ExportPathType type, string path)
    {
        Type = type;
        Path = path;
    }
}