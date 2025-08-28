namespace ContextKit.Model;

public interface ISemanticInfo
{
    string Identifier { get; }

    string Name { get; set; }

    string FullName { get; set; }

    string ShortName { get; set; }

    string Namespace { get; set; }
}
