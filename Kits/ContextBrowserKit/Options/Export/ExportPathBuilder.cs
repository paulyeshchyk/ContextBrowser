using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log;

namespace ContextBrowserKit.Options.Export;

public static class ExportPathBuilder
{
    public static ExportPaths BuildFullPath(this ExportPaths config)
    {
        var outputDirectory = Path.GetFullPath(config.OutputDirectory);

        var finalPaths = new Dictionary<ExportPathType, string>();

        foreach (var (type, relativePath) in config.RelativePaths)
        {
            finalPaths[type] = Path.Combine(outputDirectory, relativePath);
        }

        return new ExportPaths(outputDirectory, finalPaths);
    }

    public static string GetPath(this ExportPaths config, ExportPathType pathType)
    {
        var output = Path.GetFullPath(config.OutputDirectory);
        var folder = config.GetPath(pathType);
        return Path.Combine(output, folder);
    }

    public static string BuildPath(this ExportPaths config, ExportPathType pathType, string filename)
    {
        var folder = GetPath(config, pathType);
        return Path.Combine(folder, filename);
    }
}

public static class ExportPathDirectoryPreparer
{
    public static void Prepare(this ExportPaths config, OnWriteLog? onWriteLog = null)
    {
        DirectoryUtils.Prepare(config.OutputDirectory, onWriteLog);

        foreach (var path in config.GetPaths())
        {
            DirectoryUtils.Prepare(path, onWriteLog);
        }
    }
}