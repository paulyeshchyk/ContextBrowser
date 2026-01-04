using System.Collections.Generic;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

public interface IAssemblyFetcher<AssemblyReference>
    where AssemblyReference : class
{
    IEnumerable<AssemblyReference> Fetch(AssemblyPathFilterPatterns assemblyPathsFilter);
}
