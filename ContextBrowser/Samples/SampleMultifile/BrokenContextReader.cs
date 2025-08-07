namespace ContextBrowser.Samples.SampleMultifile;

// context: test.data, read
public class BrokenContextReader
{
    // context: test.data, read
    public string ReadFile()
    {
        return File.ReadAllText(string.Empty);
    }
}