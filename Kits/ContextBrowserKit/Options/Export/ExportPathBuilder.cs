using System.Collections.Generic;
using System.IO;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log;

namespace ContextBrowserKit.Options.Export;

// context:export, build
public static class ExportPathBuilder
{
    // context:export, build
    public static ExportPaths BuildFullPath1(this ExportPaths config)
    {
        var outputDirectory = config.BuildAbsolutePath(config.OutputDirectory);

        var finalPaths = new Dictionary<ExportPathType, string>();
        var outputPath = string.IsNullOrEmpty(config.CacheModel.Output) ? string.Empty : config.BuildAbsolutePath(config.CacheModel.Output);
        var inputPath = string.IsNullOrEmpty(config.CacheModel.Input) ? string.Empty : config.BuildAbsolutePath(config.CacheModel.Input);

        var finalCache = new CacheJsonModel(output: outputPath, input: inputPath, renewCache: config.CacheModel.RenewCache);

        foreach (var (type, relativePath) in config.RelativePaths)
        {
            finalPaths[type] = Path.Combine(outputDirectory, relativePath);
        }

        return new ExportFilePaths(outputDirectory, config.CacheModel, finalPaths);
    }
}

// context:export, build
public static class ExportPathDirectoryPreparer
{
    // context:export, build
    public static void Prepare(this ExportPaths config, OnWriteLog? onWriteLog = null)
    {
        DirectoryUtils.Prepare(config.BuildAbsolutePath(config.OutputDirectory), onWriteLog);

        foreach (var path in config.GetPaths())
        {
            DirectoryUtils.Prepare(config.BuildAbsolutePath(path), onWriteLog);
        }
    }
}