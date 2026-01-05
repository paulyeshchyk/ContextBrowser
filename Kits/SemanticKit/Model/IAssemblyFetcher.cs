using System.Collections.Generic;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

public interface IAssemblyFetcher<AssemblyReference>
    where AssemblyReference : class
{
    // context: assembly, read
    IEnumerable<AssemblyReference> Fetch(AssemblyPathFilterPatterns assemblyPathsFilter);
}
