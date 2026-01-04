using System.Threading;

namespace SemanticKit.Model;

public interface ISyntaxTreeWrapperBuilder<TSyntaxTreeWrapper>
    where TSyntaxTreeWrapper : ISyntaxTreeWrapper
{
    TSyntaxTreeWrapper Build(string code, string filePath, CancellationToken cancellationToken);
}
