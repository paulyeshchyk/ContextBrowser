namespace SemanticKit.Model.Options;

// pattern: configuration
// context: semantic, model
public record SemanticOptions
{
    public string ExternalNamespaceName { get; set; }

    public string FakeOwnerName { get; set; }

    public string FakeMethodName { get; set; }

    public HashSet<SemanticAccessorModifierType> MethodModifierTypes { get; set; }

    public HashSet<SemanticAccessorModifierType> ClassModifierTypes { get; set; }

    public HashSet<SemanticMemberType> MemberTypes { get; set; }

    public IEnumerable<string> CustomAssembliesPaths { get; set; }

    public bool CreateFailedCallees { get; set; }

    public SemanticOptions(string externalNamespaceName, string fakeOwnerName, string fakeMethodName, HashSet<SemanticAccessorModifierType> methodModifierTypes, HashSet<SemanticAccessorModifierType> classModifierTypes, HashSet<SemanticMemberType> memberTypes, IEnumerable<string> customAssembliesPaths, bool createFailedCallees)
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
public record CodeParsingOptions
{
    public SemanticOptions SemanticOptions { get; set; }

    public CodeParsingOptions(SemanticOptions semanticOptions)
    {
        SemanticOptions = semanticOptions;
    }
}
