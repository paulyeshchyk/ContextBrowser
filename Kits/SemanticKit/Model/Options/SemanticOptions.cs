using System;
using System.Collections.Generic;
using System.Linq;

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

    public AssemblyPathsFilter SemanticFilters { get; set; }

    /// <summary>
    /// if true then error CS8915 occured
    /// </summary>
    public bool IncludePseudoCode { get; set; }

    public SemanticOptions(
        string externalNamespaceName,
        string fakeOwnerName,
        string fakeMethodName,
        HashSet<SemanticAccessorModifierType> methodModifierTypes,
        HashSet<SemanticAccessorModifierType> classModifierTypes,
        HashSet<SemanticMemberType> memberTypes,
        IEnumerable<string> customAssembliesPaths,
        bool createFailedCallees,
        bool includePseudoCode,
        AssemblyPathsFilter semanticFilters
    )
    {
        ExternalNamespaceName = externalNamespaceName;
        FakeOwnerName = fakeOwnerName;
        FakeMethodName = fakeMethodName;
        MethodModifierTypes = methodModifierTypes;
        ClassModifierTypes = classModifierTypes;
        MemberTypes = memberTypes;
        CustomAssembliesPaths = customAssembliesPaths;
        CreateFailedCallees = createFailedCallees;
        IncludePseudoCode = includePseudoCode;
        SemanticFilters = semanticFilters;
    }
}

public record SemanticFilters
{
    public string Excluded { get; }
    public string Included { get; }

    public SemanticFilters(string excluded, string included)
    {
        Excluded = excluded;
        Included = included;
    }
}

public class AssemblyPathsFilter
{
    public readonly SemanticFilters TrustedFilters;
    public readonly SemanticFilters RuntimeFilters;
    public readonly SemanticFilters DomainFilters;

    public AssemblyPathsFilter(SemanticFilters trustedFilters, SemanticFilters runtimeFilters, SemanticFilters domainFilters)
    {
        TrustedFilters = trustedFilters;
        RuntimeFilters = runtimeFilters;
        DomainFilters = domainFilters;
    }

    /// <summary>
    /// Фильтрует пути сборок на основе доверенных фильтров.
    /// </summary>
    public IEnumerable<string> FilterTrustedPaths(IEnumerable<string> assemblyPaths)
    {
        return ApplyFilters(assemblyPaths, TrustedFilters);
    }

    /// <summary>
    /// Фильтрует пути сборок на основе фильтров времени выполнения.
    /// </summary>
    public IEnumerable<string> FilterRuntimePaths(IEnumerable<string> assemblyPaths)
    {
        return ApplyFilters(assemblyPaths, RuntimeFilters);
    }

    /// <summary>
    /// Фильтрует пути сборок на основе доменных фильтров.
    /// </summary>
    public IEnumerable<string> FilterDomainPaths(IEnumerable<string> assemblyPaths)
    {
        return ApplyFilters(assemblyPaths, DomainFilters);
    }

    /// <summary>
    /// Применяет заданные фильтры к набору путей.
    /// </summary>
    private static IEnumerable<string> ApplyFilters(IEnumerable<string> assemblyPaths, SemanticFilters filters)
    {
        var excludedPatterns = filters.Excluded.Split(';', StringSplitOptions.RemoveEmptyEntries);
        var includedPatterns = filters.Included.Split(';', StringSplitOptions.RemoveEmptyEntries);

        return assemblyPaths
            .Where(path =>
            {
                var isIncluded = includedPatterns.Length == 0 || includedPatterns.Any(p => path.Contains(p, StringComparison.OrdinalIgnoreCase));
                var isExcluded = excludedPatterns.Any(p => path.Contains(p, StringComparison.OrdinalIgnoreCase));
                return isIncluded && !isExcluded;
            })
            .ToList();
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
