namespace ContextBrowserKit.Options.Import;

// parsing: error
public record ImportOptions
{
    public string[] SearchPaths { get; }

    /// <summary>
    /// format: .cs;.cpp;
    /// </summary>
    public string FileExtensions { get; set; }

    public ImportOptions(string[] searchPaths, string fileExtensions)
    {
        SearchPaths = searchPaths;
        FileExtensions = fileExtensions;
    }
}