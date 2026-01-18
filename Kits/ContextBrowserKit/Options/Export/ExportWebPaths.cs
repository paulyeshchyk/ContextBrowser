using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ContextBrowserKit.Options.Export;

// context: file, update
public sealed class ExportWebPaths : ExportPaths
{
    public ExportWebPaths() : base()
    {
    }

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

    // context: file, update
    public override string BuildRelativePath(string from, params string[] files)
    {
        string relativePathFromFiles = string.Join("/", files);

        string cleanedFrom = from.Replace("\\", "/").Replace("//", "/").TrimStart('.', '/');
        string baseUrl = $"./{cleanedFrom}";

        string cleanedRelativePath = relativePathFromFiles.Replace("\\", "/").Replace("//", "/").TrimStart('.', '/');

        return $"{baseUrl}/{cleanedRelativePath}";
    }
}
