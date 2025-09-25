using System.Collections.Generic;
using System.IO;

namespace UmlKit.PlantUmlSpecification;

public record UmlWriteOptions
{
    public int AlignMaxWidth { get; set; }

    public int DepthIndex { get; set; }

    public UmlWriteOptions(int alignMaxWidth, int depthIndex = 0)
    {
        AlignMaxWidth = alignMaxWidth;
        DepthIndex = depthIndex;
    }
}

// context: model, uml
// pattern: Composite
public interface IUmlWritable
{
    void WriteTo(TextWriter writer, UmlWriteOptions writeOptions);
}

// context: model, uml
// pattern: Composite
public interface IUmlElement : IUmlWritable
{
}

public interface IUmlElementCollection
{
    SortedList<int, IUmlElement> Elements { get; }
}

public interface IUmlDeclarable
{
    string Declaration { get; }

    string Alias { get; }
}

public interface IUmlParticipant : IUmlElement, IUmlDeclarable
{
}

public interface IUmlTransition<TObject> : IUmlElement
    where TObject : IUmlParticipant
{
    public TObject From { get; }

    public TObject To { get; }
}

public class UmlDeclarableDto : IUmlDeclarable
{
    public string Declaration { get; }

    public string Alias { get; }

    public UmlDeclarableDto(string declaration, string shortName)
    {
        Declaration = declaration;
        Alias = shortName;
    }
}