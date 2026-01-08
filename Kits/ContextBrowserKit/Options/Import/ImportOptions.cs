using System;

namespace ContextBrowserKit.Options.Import;

// parsing: error
public record ImportOptions
{
    public string[] SearchPaths { get; }

    /// <summary>
    /// format: obj/**;**/*Tests*
    /// </summary>
    public string Exclude { get; }

    /// <summary>
    /// format: .cs;.cpp;
    /// </summary>
    public string FileExtensions { get; set; }

    public ImportOptions(string[] searchPaths, string fileExtensions, string exclude)
    {
        SearchPaths = searchPaths;
        FileExtensions = fileExtensions;
        Exclude = exclude;
    }
}