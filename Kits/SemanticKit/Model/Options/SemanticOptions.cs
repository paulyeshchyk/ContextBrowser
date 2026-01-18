using System;
using System.Collections.Generic;
using ContextBrowserKit.Commandline.Polyfills;

namespace SemanticKit.Model.Options;

// pattern: configuration
// context: semantic, model
public record SemanticOptions
{
    [CommandLineArgument("externalNaming", "External Naming")]
    public SemanticExternalNaming ExternalNaming { get; set; }

    public SemanticFakeNaming FakeNaming { get; set; }

    public HashSet<SemanticAccessorModifierType> MethodModifierTypes { get; set; }

    public HashSet<SemanticAccessorModifierType> ClassModifierTypes { get; set; }

    public HashSet<SemanticMemberType> MemberTypes { get; set; }

    public IEnumerable<string> CustomAssembliesPaths { get; set; }

    public bool CreateFailedCallees { get; set; }

    public AssemblyPathFilterPatterns SemanticFilters { get; set; }

    public string GlobalUsings { get; set; }

    /// <summary>
    /// задаёт к-во потоков, используемых при парсинге файлов
    /// </summary>
    public int MaxDegreeOfParallelism { get; set; } = -1;

    /// <summary>
    /// if true then error CS8915 occured
    /// </summary>
    public bool IncludePseudoCode { get; set; }

    public SemanticOptions(
        SemanticExternalNaming externalNaming,
        SemanticFakeNaming fakeNaming,
        HashSet<SemanticAccessorModifierType> methodModifierTypes,
        HashSet<SemanticAccessorModifierType> classModifierTypes,
        HashSet<SemanticMemberType> memberTypes,
        IEnumerable<string> customAssembliesPaths,
        bool createFailedCallees,
        bool includePseudoCode,
        string globalUsings,
        AssemblyPathFilterPatterns semanticFilters,
        int maxDegreeOfParallelism)
    {
        MethodModifierTypes = methodModifierTypes;
        ClassModifierTypes = classModifierTypes;
        MemberTypes = memberTypes;
        CustomAssembliesPaths = customAssembliesPaths;
        CreateFailedCallees = createFailedCallees;
        IncludePseudoCode = includePseudoCode;
        GlobalUsings = globalUsings;
        SemanticFilters = semanticFilters;
        MaxDegreeOfParallelism = maxDegreeOfParallelism;
        ExternalNaming = externalNaming;
        FakeNaming = fakeNaming;
    }
}

public record SemanticExternalNaming
{
    public string NamespaceName { get; set; }

    public string ClassName { get; set; }

    public string MethodName { get; set; }

    public string ResultTypeName { get; set; }

    public SemanticExternalNaming(string namespaceName, string className, string methodName, string resultTypeName)
    {
        NamespaceName = namespaceName;
        ClassName = className;
        MethodName = methodName;
        ResultTypeName = resultTypeName;
    }
}

public record SemanticFakeNaming
{
    public string OwnerName { get; set; }

    public string MethodName { get; set; }

    public string NamespaceName { get; set; }

    public string ClassName { get; set; }

    public string ResultTypeName { get; set; }

    public SemanticFakeNaming(string ownerName, string methodName, string namespaceName, string className, string resultTypeName)
    {
        OwnerName = ownerName;
        MethodName = methodName;
        NamespaceName = namespaceName;
        ClassName = className;
        ResultTypeName = resultTypeName;
    }
}

// parsing: error
// context: model, semantic
public record CodeParsingOptions(SemanticOptions SemanticOptions, string SemanticLanguage)
{
    [CommandLineArgument("semanticLanguage", "Language semantic")]
    public string SemanticLanguage { get; set; } = SemanticLanguage;

    [CommandLineArgument("semanticOptions", "Language semantic options")]
    public SemanticOptions SemanticOptions { get; set; } = SemanticOptions;
}
