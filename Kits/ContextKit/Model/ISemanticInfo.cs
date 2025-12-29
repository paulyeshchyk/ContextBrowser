namespace ContextKit.Model;

public interface ISemanticInfo
{
    string Identifier { get; }

    string Name { get; }

    string FullName { get; }

    string ShortName { get; }

    string Namespace { get; }
}
