namespace UmlKit.Model;

public class UmlArrow : IUmlElement
{
    public void WriteTo(TextWriter writer)
    {
        var value = "-->";

        writer.Write(value);
    }
}