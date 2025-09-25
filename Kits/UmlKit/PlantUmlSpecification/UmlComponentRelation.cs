using System.IO;

namespace UmlKit.PlantUmlSpecification;

public class UmlComponentRelation : IUmlElement
{
    private readonly string _leftObject;
    private readonly string _rightObject;
    private readonly UmlJoin _join;

    public UmlComponentRelation(string leftObject, string rightObject, UmlArrowDirection relationDirection, string relationVisibility)
    {
        _leftObject = leftObject;
        _rightObject = rightObject;
        _join = new UmlJoin(relationDirection, relationVisibility);
    }

    public void WriteTo(TextWriter writer, UmlWriteOptions writeOptions)
    {
        writer.WriteLine();
        writer.Write($"\"{_leftObject}\"");
        _join.WriteTo(writer, writeOptions);
        writer.Write($"\"{_rightObject}\"");
    }
}
