using System.IO;

namespace UmlKit.Model;

public class UmlLine : IUmlElement
{
    public readonly string Text;

    public UmlLine(string text)
    {
        Text = text;
    }

    public void WriteTo(TextWriter writer, int alignNameMaxWidth)
    {
        writer.WriteLine(Text);
    }
}