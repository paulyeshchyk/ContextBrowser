using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

public interface ISemanticMapExtractor
{
    Task<SemanticCompilationMap> CreateSemanticMapFromFilesAsync(SemanticOptions options, IEnumerable<string> codeFiles, CancellationToken cancellationToken);
    Task<SemanticCompilationMap> CreateSemanticMapFromFilesAsync(SemanticOptions options, string filePath, CancellationToken cancellationToken);
}

public interface ICompilationBuilder : ISemanticMapExtractor
{
    ICompilationWrapper Build(SemanticOptions options, IEnumerable<ISyntaxTreeWrapper> syntaxTrees, IEnumerable<string> customAssembliesPaths, string name, CancellationToken cancellationToken);
}
