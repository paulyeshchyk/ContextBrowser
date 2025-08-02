namespace ContextBrowser.Samples.Test.Multifile;

// context: data, read
public class BrokenContextReader
{
    // context: data, read
    public string ReadFile()
    {
        return File.ReadAllText(string.Empty);
    }
}