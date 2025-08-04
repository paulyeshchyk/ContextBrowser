namespace ContextBrowser.Samples.SampleMultifile;

// context: create, build, sample.broken
public class BrokenOrchestra
{
    public readonly BrokenContextReader Reader = new BrokenContextReader();

    // context: validate, sample.broken
    public bool ValidateInput(string raw)
    {
        return !string.IsNullOrWhiteSpace(raw);
    }

    //context: read, _fakeAction
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