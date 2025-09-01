using System.IO;

namespace ContextBrowser.Samples.SampleMultifile;

// context: S5, read
public class BrokenContextReader
{
    // context: S5, read
    public string ReadFile()
    {
        return File.ReadAllText(string.Empty);
    }
}