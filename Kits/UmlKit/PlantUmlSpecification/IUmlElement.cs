using System.Collections.Generic;
using System.IO;

namespace UmlKit.Model;

// context: model, uml
// pattern: Composite
public interface IUmlElement
{
    void WriteTo(TextWriter writer, int alignNameMaxWidth);
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