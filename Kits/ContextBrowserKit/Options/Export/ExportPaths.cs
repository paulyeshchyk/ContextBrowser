namespace ContextBrowserKit.Options.Export;

public record ExportPaths
{
    public string OutputDirectory { get; set; }

    public IReadOnlyDictionary<ExportPathType, string> RelativePaths { get; set; }

    public ExportPaths(string outputDirectory, IReadOnlyDictionary<ExportPathType, string> paths)
    {
        OutputDirectory = outputDirectory;
        RelativePaths = paths;
    }

    public ExportPaths(string outputDirectory, params ExportPathItem[] paths)
    {
        OutputDirectory = outputDirectory;

        var d = new Dictionary<ExportPathType, string>();
        foreach(var pathItem in paths)
        {
            d[pathItem.Type] = pathItem.Path;
        }
        RelativePaths = d;
    }

    public string GetPath(ExportPathType type)
    {
        if(RelativePaths.TryGetValue(type, out var path)) return path;
        throw new InvalidOperationException($"Export path not found for {type}");
    }

    public ExportPathType[] GetPathTypes()
    {
        return RelativePaths.Keys.ToArray();
    }

    public string[] GetPaths()
    {
        return RelativePaths.Values.ToArray();
    }
}

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