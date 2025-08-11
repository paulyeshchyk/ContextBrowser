namespace RoslynKit.Model;

// pattern: configuration
// context: csharp, model
public record RoslynCodeParserOptions(
    string ExternalNamespaceName,
    string FakeOwnerName,
    string FakeMethodName,
    HashSet<RoslynCodeParserAccessorModifierType> MethodModifierTypes,
    HashSet<RoslynCodeParserAccessorModifierType> ClassModifierTypes,
    HashSet<RoslynCodeParserMemberType> MemberTypes,
    IEnumerable<string> CustomAssembliesPaths,
    bool CreateFailedCallees)
{
}

public record RoslynOptions(
    string theSourcePath,
    RoslynCodeParserOptions roslynCodeparserOptions)
{
}
