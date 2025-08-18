namespace ContextBrowserKit.Options.Export;

// parsing: error
public record ExportOptions
{
    public ExportMatrixOptions ExportMatrix { get; set; }

    public ExportPaths Paths { get; set; }

    public ExportOptions(ExportMatrixOptions exportMatrix, ExportPaths paths)
    {
        ExportMatrix = exportMatrix;
        Paths = paths;
    }
}
