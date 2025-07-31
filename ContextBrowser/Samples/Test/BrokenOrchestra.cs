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
        var data = Reader.ReadFile();
        if(ValidateInput(data))
        {
            return;
        }
        else
        {
            throw new Exception(string.Empty);
        }
    }
}

// context: data, read
public class BrokenContextReader
{
    // context: data, read
    public string ReadFile()
    {
        return File.ReadAllText(string.Empty);
    }
}