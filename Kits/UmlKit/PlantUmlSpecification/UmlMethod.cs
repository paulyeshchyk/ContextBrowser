using System.IO;

namespace UmlKit.Model;

public class UmlMethod : IUmlElement
{
    public readonly string Text;

    public UmlMethod(string text)
    {
        Text = text;
    }

    public void WriteTo(TextWriter writer)
    {
        writer.WriteLine(Text);
    }
}