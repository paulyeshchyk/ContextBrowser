using System;
using System.IO;

namespace UmlKit.Model;

public class UmlLayoutDirection : IUmlElement, IUmlWritable
{
    public enum Direction
    {
        LeftToRight,
        TopToBottom,
        RightToLeft,
        BottomToTop,
    }

    private readonly Direction _direction;

    public UmlLayoutDirection(Direction direction)
    {
        _direction = direction;
    }

    public void WriteTo(TextWriter writer, UmlWriteOptions writeOptions)
    {
        writer.WriteLine();
        string line = _direction switch
        {
            Direction.LeftToRight => "left to right direction",
            Direction.TopToBottom => "top to bottom direction",
            Direction.BottomToTop => "bottom to top direction",
            Direction.RightToLeft => "right to left direction",
            _ => throw new NotImplementedException()
        };
        writer.WriteLine(line);
        writer.WriteLine();
    }
}
