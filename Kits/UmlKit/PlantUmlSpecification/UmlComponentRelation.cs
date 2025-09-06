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

    public void WriteTo(TextWriter writer)
    {
        writer.WriteLine();
        writer.Write($"\"{_leftObject}\"");
        _join.WriteTo(writer);
        writer.Write($"\"{_rightObject}\"");
    }
}

public class UmlJoin : IUmlElement
{
    private readonly UmlArrowDirection _direction;
    private readonly string _style;

    public UmlJoin(UmlArrowDirection direction, string style)
    {
        _direction = direction;
        _style = style;
    }

    public void WriteTo(TextWriter writer)
    {
        var theStyle = string.Empty;
        if (!string.IsNullOrEmpty(_style))
        {
            theStyle = $"[{_style}]";
        }
        switch (_direction)
        {
            case UmlArrowDirection.ToLeft:
                writer.Write($"<-{theStyle}-");
                break;
            case UmlArrowDirection.ToRight:
                writer.Write($"-{theStyle}->");
                break;
            case UmlArrowDirection.None:
                writer.Write($"-{theStyle}-");
                break;
            default:
                throw new NotImplementedException();
        }
    }
}