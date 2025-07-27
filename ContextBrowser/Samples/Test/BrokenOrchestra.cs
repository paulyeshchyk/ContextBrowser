namespace ContextBrowser.Samples.Test;

// context: create, basic
public class BrokenOrchestra
{
    public readonly BrokenContextReader Reader = new BrokenContextReader();

    // context: validate, basic
    public bool ValidateInput(string raw)
    {
        return !string.IsNullOrWhiteSpace(raw);
    }

    //context: basic, read
    public void Test()
    {
        Reader.ReadFile();
    }
}

// context: data, read
public class BrokenContextReader
{
    // context: data, read
    public void ReadFile()
    {
        File.ReadAllText(string.Empty);
    }
}