using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

public interface ICompilationBuilder
{
    ICompilationWrapper BuildCompilation(SemanticOptions options, IEnumerable<ISyntaxTreeWrapper> syntaxTrees, IEnumerable<string> customAssembliesPaths, string name);

    SemanticCompilationMap BuildCompilationMap(SemanticOptions options, IEnumerable<string> codeFiles, CancellationToken cancellationToken = default);
}
