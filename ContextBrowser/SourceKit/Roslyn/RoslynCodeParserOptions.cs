namespace ContextBrowser.SourceKit.Roslyn;

// pattern: configuration
public record RoslynCodeParserOptions(
    HashSet<RoslynCodeParserAccessorModifierType> MethodModifierTypes,
    HashSet<RoslynCodeParserAccessorModifierType> ClassModifierTypes,
    HashSet<RoslynCodeParserMemberType> MemberTypes)
{
    public static RoslynCodeParserOptions Default => new(

        MethodModifierTypes: new() { RoslynCodeParserAccessorModifierType.@public, RoslynCodeParserAccessorModifierType.@protected, RoslynCodeParserAccessorModifierType.@internal },
        ClassModifierTypes: new() { RoslynCodeParserAccessorModifierType.@public, RoslynCodeParserAccessorModifierType.@protected, RoslynCodeParserAccessorModifierType.@internal },
        MemberTypes: new() { RoslynCodeParserMemberType.@enum,
                             RoslynCodeParserMemberType.@class,
                             RoslynCodeParserMemberType.@record,
                             RoslynCodeParserMemberType.@struct }
    );
}
