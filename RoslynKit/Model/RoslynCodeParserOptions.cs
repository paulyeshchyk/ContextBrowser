namespace RoslynKit.Model;

// pattern: configuration
// context: csharp, model
public record RoslynCodeParserOptions(
    HashSet<RoslynCodeParserAccessorModifierType> MethodModifierTypes,
    HashSet<RoslynCodeParserAccessorModifierType> ClassModifierTypes,
    HashSet<RoslynCodeParserMemberType> MemberTypes,
    IEnumerable<string> CustomAssembliesPaths,
    bool CreateFailedCallees,
    bool ShowForeignInstancies)
{
}