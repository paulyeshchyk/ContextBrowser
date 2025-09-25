using System.IO;

namespace UmlKit.PlantUmlSpecification;

public class UmlStyle : IUmlElement
{
    public string Name { get; }

    public UmlStyleAttributes Attributes { get; }

    public UmlStyle(string name, UmlStyleAttributes attributes)
    {
        Name = name;
        Attributes = attributes;
    }

    public void WriteTo(TextWriter writer, UmlWriteOptions writeOptions)
    {
        writer.WriteLine($".{Name} {{");
        Attributes.WriteTo(writer, writeOptions);
        writer.WriteLine("}");
    }

    public static UmlStyleAttributesBuilder Builder(string name) => new UmlStyleAttributesBuilder(name);
}
