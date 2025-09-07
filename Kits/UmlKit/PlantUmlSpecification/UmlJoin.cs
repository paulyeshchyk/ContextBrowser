using System;
using System.IO;
using UmlKit.Model;

namespace UmlKit.PlantUmlSpecification;

public class UmlJoin : IUmlElement
{
    private readonly UmlArrowDirection _direction;
    private readonly string _style;

    public UmlJoin(UmlArrowDirection direction, string style)
    {
        _direction = direction;
        _style = style;
    }

    public void WriteTo(TextWriter writer, int alignNameMaxWidth)
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