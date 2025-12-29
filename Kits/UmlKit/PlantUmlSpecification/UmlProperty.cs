using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UmlKit.PlantUmlSpecification;

public class UmlProperty : IUmlElement
{
    public readonly string Text;
    public readonly UmlMemberVisibility Visibility;
    public readonly string? Url;

    public UmlProperty(string text, UmlMemberVisibility visibility, string? url)
    {
        Text = text;
        Visibility = visibility;
        Url = url;
    }

    public void WriteTo(TextWriter writer, UmlWriteOptions writeOptions)
    {
        List<string?> properties = new();
        properties.Add(Visibility.ToUmlString());
        properties.Add(Text);

        //properties.Add(MethodAttributesBuilder.BuildUrl(Url));

        var result = string.Join(" ", properties.Cast<string>());
        writer.WriteLine(result);
    }
}