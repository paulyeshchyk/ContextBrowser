using System.IO;

namespace UmlKit.Model;

public class UmlComment : IUmlElement
{
    public readonly string Text;

    public UmlComment(string text)
    {
        Text = text;
    }

    public void WriteTo(TextWriter writer, UmlWriteOptions writeOptions)
    {
        writer.WriteLine();
        writer.WriteLine($"' {Text}");
    }
}