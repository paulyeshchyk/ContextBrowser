using ContextKit.Model;

namespace ContextKit.ContextData;

public interface IUmlUrlBuilder
{
    string? BuildActionUrl(string? action);
    string? BuildContextInfoUrl(IContextInfo? contextInfo);
    string? BuildDomainUrl(string? domain);
    string? BuildNamespaceUrl(string? nameSpace);
}
