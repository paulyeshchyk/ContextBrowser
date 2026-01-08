using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;

namespace SemanticKit.Model;

public interface ISymbolWrapperConverter
{
    Task<ISymbolInfo> ConvertAsync(ISemanticModelWrapper semanticModel, ISyntaxNodeWrapper syntaxWrapper, CancellationToken cancellationToken);
}
