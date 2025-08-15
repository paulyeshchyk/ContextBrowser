namespace ExporterKit.Options;

// parsing: error
public record ExportOptions
{
    public ExportMatrixOptions ExportMatrix { get; set; }

    public string OutputDirectory { get; set; }

    public ExportOptions(ExportMatrixOptions exportMatrix, string outputDirectory)
    {
        ExportMatrix = exportMatrix;
        OutputDirectory = outputDirectory;
    }
}