using System;
using System.IO;

namespace UmlKit.Model;

public class UmlLayoutDirection : IUmlElement
{
    public enum Direction
    {
        LeftToRight,
        TopToBottom
    }
    private readonly Direction _direction;

    public UmlLayoutDirection(Direction direction)
    {
        _direction = direction;
    }
    public void WriteTo(TextWriter writer)
    {
        writer.WriteLine();
        string line = _direction switch
        {
            Direction.LeftToRight => "left to right direction",
            Direction.TopToBottom => "top to bottom direction",
            _ => throw new NotImplementedException()
        };
        writer.WriteLine(line);
        writer.WriteLine();
    }

}
