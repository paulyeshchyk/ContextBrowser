using System.Threading;
using System.Threading.Tasks;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

public interface ICodeInjector
{
    Task<string> ReadAndInjectPseudoCodeAsync(SemanticOptions options, string filePath, CancellationToken cancellationToken);
}