namespace RoslynKit.Model;

// pattern: configuration
// context: csharp, model
public record RoslynCodeParserOptions
{
    public string ExternalNamespaceName { get; set; }

    public string FakeOwnerName { get; set; }

    public string FakeMethodName { get; set; }

    public HashSet<RoslynCodeParserAccessorModifierType> MethodModifierTypes { get; set; }

    public HashSet<RoslynCodeParserAccessorModifierType> ClassModifierTypes { get; set; }

    public HashSet<RoslynCodeParserMemberType> MemberTypes { get; set; }

    public IEnumerable<string> CustomAssembliesPaths { get; set; }

    public bool CreateFailedCallees { get; set; }

    public RoslynCodeParserOptions(string externalNamespaceName, string fakeOwnerName, string fakeMethodName, HashSet<RoslynCodeParserAccessorModifierType> methodModifierTypes, HashSet<RoslynCodeParserAccessorModifierType> classModifierTypes, HashSet<RoslynCodeParserMemberType> memberTypes, IEnumerable<string> customAssembliesPaths, bool createFailedCallees)
    {
        ExternalNamespaceName = externalNamespaceName;
        FakeOwnerName = fakeOwnerName;
        FakeMethodName = fakeMethodName;
        MethodModifierTypes = methodModifierTypes;
        ClassModifierTypes = classModifierTypes;
        MemberTypes = memberTypes;
        CustomAssembliesPaths = customAssembliesPaths;
        CreateFailedCallees = createFailedCallees;
    }
}

// parsing: error
public record RoslynOptions
{
    public string SourcePath { get; set; }

    public RoslynCodeParserOptions RoslynCodeParser { get; set; }

    public RoslynOptions(string sourcePath, RoslynCodeParserOptions roslynCodeParser)
    {
        SourcePath = sourcePath;
        RoslynCodeParser = roslynCodeParser;
    }
}
