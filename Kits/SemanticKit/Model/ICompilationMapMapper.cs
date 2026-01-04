using System.Collections.Generic;

namespace SemanticKit.Model;

public interface ICompilationMapMapper<TSyntaxTreeWrapper>
    where TSyntaxTreeWrapper : ISyntaxTreeWrapper
{
    SemanticCompilationMap<TSyntaxTreeWrapper> MapSemanticModelToCompilationMap(IEnumerable<TSyntaxTreeWrapper> syntaxTrees, ICompilationWrapper compilation);
}
