namespace ContextBrowser.ContextKit.Roslyn;

// context: csharp, model
// pattern: configuration
internal record RoslynParserOptions(
    HashSet<RoslynAccessorModifierType> MethodModifierTypes,
    HashSet<RoslynAccessorModifierType> ClassModifierTypes,
    HashSet<RoslynMemberType> MemberTypes)
{
    public static RoslynParserOptions Default => new(
        MethodModifierTypes: new() { RoslynAccessorModifierType.@public, RoslynAccessorModifierType.@protected, RoslynAccessorModifierType.@internal },
        ClassModifierTypes: new() { RoslynAccessorModifierType.@public, RoslynAccessorModifierType.@protected, RoslynAccessorModifierType.@internal },
        MemberTypes: new() { RoslynMemberType.@enum,
                             RoslynMemberType.@class,
                             RoslynMemberType.@record,
                             RoslynMemberType.@struct }
    );
}
