using System.IO;

namespace ContextSamples.ContextSamples.S5;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Пометьте члены как статические", Justification = "<Ожидание>")]
// context: S5, read
public class BrokenContextReader
{
    // context: S5, read
    public string ReadFile()
    {
        return File.ReadAllText(string.Empty);
    }
}