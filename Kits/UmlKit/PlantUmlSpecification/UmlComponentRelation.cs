using System;
using System.IO;
using UmlKit.Model;

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

    public void WriteTo(TextWriter writer, int alignNameMaxWidth)
    {
        writer.WriteLine();
        writer.Write($"\"{_leftObject}\"");
        _join.WriteTo(writer, alignNameMaxWidth);
        writer.Write($"\"{_rightObject}\"");
    }
}
