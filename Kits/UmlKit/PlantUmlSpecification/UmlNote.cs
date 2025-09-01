using System;
using System.IO;

namespace UmlKit.Model;

public class UmlNote : IUmlElement
{
    private readonly string _note;
    private readonly UmlNotePosition _position;
    private readonly string _caller;

    public UmlNote(string caller, UmlNotePosition position, string note)
    {
        _note = note;
        _position = position;
        _caller = caller;
    }

    public void WriteTo(TextWriter writer)
    {
        writer.WriteLine();
        writer.WriteLine($"note {_position.ToUmlString()} {_caller}");
        writer.WriteLine(_note);
        writer.WriteLine("end note");
        writer.WriteLine();
    }
}

public enum UmlNotePosition
{
    Left,
    Right,
    Over
}

internal static class UmlNotePositionExt
{
    public static string ToUmlString(this UmlNotePosition position)
    {
        string positionString = position switch
        {
            UmlNotePosition.Left => "left",
            UmlNotePosition.Right => "right",
            UmlNotePosition.Over => "over",
            _ => throw new NotImplementedException(),
        };
        return positionString;
    }
}