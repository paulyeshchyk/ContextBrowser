using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ContextBrowserKit.Options.Export;

// context: file, update
public sealed class ExportFilePaths : ExportPaths
{
    public ExportFilePaths() : base()
    {
    }

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

    public override string BuildRelativePath(string from, params string[] files)
    {
        string filesPath = Path.Combine(files);

        string combinedPath = Path.Combine(from, filesPath);

        return combinedPath;
    }
}
