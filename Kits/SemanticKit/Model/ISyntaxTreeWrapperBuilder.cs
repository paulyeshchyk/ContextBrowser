using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace SemanticKit.Model;

public interface ISyntaxTreeWrapperBuilder<TSyntaxTreeWrapper>
    where TSyntaxTreeWrapper : ISyntaxTreeWrapper
{
    Task<TSyntaxTreeWrapper> BuildAsync(string code, string filePath, CancellationToken cancellationToken);
}
