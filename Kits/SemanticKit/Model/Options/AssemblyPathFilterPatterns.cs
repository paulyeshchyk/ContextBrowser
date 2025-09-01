using ContextBrowserKit.Filters;

namespace SemanticKit.Model.Options;

public class AssemblyPathFilterPatterns
{
    public readonly FilterPatterns TrustedFilters;
    public readonly FilterPatterns RuntimeFilters;
    public readonly FilterPatterns DomainFilters;

    public AssemblyPathFilterPatterns(FilterPatterns trustedFilters, FilterPatterns runtimeFilters, FilterPatterns domainFilters)
    {
        TrustedFilters = trustedFilters;
        RuntimeFilters = runtimeFilters;
        DomainFilters = domainFilters;
    }
}