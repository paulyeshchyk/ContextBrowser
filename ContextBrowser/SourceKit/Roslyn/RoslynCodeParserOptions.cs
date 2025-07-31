namespace ContextBrowser.SourceKit.Roslyn;

// pattern: configuration
// context: csharp, model
public record RoslynCodeParserOptions(
    HashSet<RoslynCodeParserAccessorModifierType> MethodModifierTypes,
    HashSet<RoslynCodeParserAccessorModifierType> ClassModifierTypes,
    HashSet<RoslynCodeParserMemberType> MemberTypes,
    IEnumerable<string> CustomAssembliesPaths)
{
    // context: csharp, create
    public static RoslynCodeParserOptions Default(IEnumerable<string>? paths = default) => new(

        MethodModifierTypes: new()
        {
            RoslynCodeParserAccessorModifierType.@public,
            RoslynCodeParserAccessorModifierType.@protected,
            RoslynCodeParserAccessorModifierType.@internal
        },
        ClassModifierTypes: new()
        {
            RoslynCodeParserAccessorModifierType.@public,
            RoslynCodeParserAccessorModifierType.@protected,
            RoslynCodeParserAccessorModifierType.@internal
        },
        MemberTypes: new()
        {
            RoslynCodeParserMemberType.@enum,
            RoslynCodeParserMemberType.@class,
            RoslynCodeParserMemberType.@interface,
            RoslynCodeParserMemberType.@record,
            RoslynCodeParserMemberType.@struct
        },
        CustomAssembliesPaths: paths ?? Enumerable.Empty<string>()
    );
}
