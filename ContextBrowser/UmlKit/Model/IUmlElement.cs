namespace ContextBrowser.UmlKit.Model;

// context: model, uml
// pattern: Composite
public interface IUmlElement
{
    void WriteTo(TextWriter writer);
}

public interface IUmlDeclarable
{
    string Declaration { get; }

    string ShortName { get; }
}
