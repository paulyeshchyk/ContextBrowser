namespace ContextBrowser.Samples.SampleMultifile;

// context: create, build, test.broken
public class BrokenOrchestra
{
    public readonly BrokenContextReader Reader = new BrokenContextReader();

    // context: build, test.broken
    public bool ValidateInput(string raw)
    {
        return !string.IsNullOrWhiteSpace(raw);
    }

    //context: read, test.broken
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