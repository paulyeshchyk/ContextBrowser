using System.IO;

namespace ContextSamples.ContextSamples.S5;

// context: S5, read
public class BrokenContextReader
{
    // context: S5, read
    public string ReadFile()
    {
        return File.ReadAllText(string.Empty);
    }
}