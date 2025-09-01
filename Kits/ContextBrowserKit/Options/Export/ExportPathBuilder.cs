using System.Collections.Generic;
using System.IO;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log;

namespace ContextBrowserKit.Options.Export;

// context:export, build
public static class ExportPathBuilder
{
    // context:export, build
    public static ExportPaths BuildFullPath(this ExportPaths config)
    {
        var outputDirectory = Path.GetFullPath(config.OutputDirectory);

        var finalPaths = new Dictionary<ExportPathType, string>();
        var outputPath = string.IsNullOrEmpty(config.CacheModel.Output) ? string.Empty : Path.GetFullPath(config.CacheModel.Output);
        var inputPath = string.IsNullOrEmpty(config.CacheModel.Input) ? string.Empty : Path.GetFullPath(config.CacheModel.Input);

        var finalCache = new CacheJsonModel(output: outputPath, input: inputPath, renewCache: config.CacheModel.RenewCache);

        foreach (var (type, relativePath) in config.RelativePaths)
        {
            finalPaths[type] = Path.Combine(outputDirectory, relativePath);
        }

        return new ExportPaths(outputDirectory, config.CacheModel, finalPaths);
    }

    // context:export, build
    public static string GetPath(this ExportPaths config, ExportPathType pathType)
    {
        var output = Path.GetFullPath(config.OutputDirectory);
        var folder = config.GetPath(pathType);
        return Path.Combine(output, folder);
    }

    // context:export, build
    public static string BuildPath(this ExportPaths config, ExportPathType pathType, string filename)
    {
        var folder = GetPath(config, pathType);
        return Path.Combine(folder, filename);
    }
}

// context:export, build
public static class ExportPathDirectoryPreparer
{
    // context:export, build
    public static void Prepare(this ExportPaths config, OnWriteLog? onWriteLog = null)
    {
        DirectoryUtils.Prepare(config.OutputDirectory, onWriteLog);

        foreach (var path in config.GetPaths())
        {
            DirectoryUtils.Prepare(path, onWriteLog);
        }
    }
}