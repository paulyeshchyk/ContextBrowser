namespace UmlKit.Model;

// context: model, uml
// pattern: Composite
public interface IUmlElement
{
    void WriteTo(TextWriter writer);
}

public interface IUmlDeclarable
{
    string Declaration { get; }

    string Alias { get; }
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