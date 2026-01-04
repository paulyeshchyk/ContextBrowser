using System.Collections.Generic;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

public interface ISyntaxCompiler<TMetadataReference, TSyntaxTreeWrapper, TCompilation>
    where TSyntaxTreeWrapper : ISyntaxTreeWrapper
{
    TCompilation CreateCompilation(SemanticOptions options, IEnumerable<TSyntaxTreeWrapper> syntaxTrees, string name, IEnumerable<TMetadataReference> referencesToLoad);
}
