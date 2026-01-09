using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

// context: semantic, model
public interface ISemanticCompilationMapBuilder<TSyntaxTree, TSemanticModel>
    where TSyntaxTree : ISyntaxTreeWrapper
    where TSemanticModel : notnull
{
    // context: semantic, build
    Task<SemanticCompilationMap<TSyntaxTree>> BuildCompilationMapAsync(IEnumerable<string> codeFiles, string compilationName, CancellationToken cancellationToken);
}

public interface ISemanticCompilationViewBuilder<TSyntaxTree, TSemanticModel>
    where TSyntaxTree : ISyntaxTreeWrapper
    where TSemanticModel : notnull
{
    // context: semantic, build
    Task<SemanticCompilationView> BuildCompilationViewAsync(string code, string filePath, string compilationName, CancellationToken cancellationToken);
}