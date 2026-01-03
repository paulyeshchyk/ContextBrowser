using System.Threading;
using ContextKit.Model;

namespace SemanticKit.Model;

public interface ISymbolWrapperConverter
{
    CSharpISymbolWrapper Convert(ISemanticModelWrapper semanticModel, ISyntaxNodeWrapper syntaxWrapper, CancellationToken cancellationToken);
}
