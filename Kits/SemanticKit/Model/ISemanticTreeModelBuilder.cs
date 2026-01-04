using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

// context: semantic, model
public interface ISemanticTreeModelBuilder<TSyntaxTree, TSemanticModel>
    where TSyntaxTree : notnull
    where TSemanticModel : notnull
{
    // context: semantic, build
    public SemanticCompilationView BuildCompilationView(string code, string filePath, SemanticOptions options, CancellationToken cancellationToken);

    // context: semantic, build
    public Task<SemanticCompilationMap> BuildCompilationMapAsync(IEnumerable<string> codeFiles, SemanticOptions options, CancellationToken cancellationToken);
}