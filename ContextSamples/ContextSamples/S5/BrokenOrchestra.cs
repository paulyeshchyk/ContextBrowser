using System;

namespace ContextBrowser.Samples.SampleMultifile;

// context: create, build, S5.1
public class BrokenOrchestra
{
    public readonly BrokenContextReader Reader = new BrokenContextReader();

    // context: build, S5.1
    public bool ValidateInput(string raw)
    {
        return !string.IsNullOrWhiteSpace(raw);
    }

    //context: read, S5.1
    public void Test()
    {
        var data = Reader.ReadFile();
        if (ValidateInput(data))
        {
            return;
        }
        else
        {
            throw new Exception(string.Empty);
        }
    }
}