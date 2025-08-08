namespace RoslynKit.Model;

// pattern: configuration
// context: csharp, model
public record RoslynCodeParserOptions(
    string FakeNamespaceName,
    string FakeOwnerName,
    string FakeMethodName,
    HashSet<RoslynCodeParserAccessorModifierType> MethodModifierTypes,
    HashSet<RoslynCodeParserAccessorModifierType> ClassModifierTypes,
    HashSet<RoslynCodeParserMemberType> MemberTypes,
    IEnumerable<string> CustomAssembliesPaths,
    bool CreateFailedCallees)
{
}