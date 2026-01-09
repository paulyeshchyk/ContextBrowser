using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options;
using LoggerKit;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

// context: roslyn, build, contextInfo
public class SemanticCompilationMapBuilder<TSyntaxTreeWrapper> : ISemanticCompilationMapBuilder<TSyntaxTreeWrapper, ISemanticModelWrapper>
    where TSyntaxTreeWrapper : ISyntaxTreeWrapper
{
    private readonly ISemanticModelStorage<TSyntaxTreeWrapper, ISemanticModelWrapper> _modelStorage;
    private readonly ISemanticMapExtractor<TSyntaxTreeWrapper> _semanticMapBuilder;

    public SemanticCompilationMapBuilder(
        ISemanticModelStorage<TSyntaxTreeWrapper, ISemanticModelWrapper> modelStorage,
        ISemanticMapExtractor<TSyntaxTreeWrapper> semanticMapBuilder)
    {
        _modelStorage = modelStorage;
        _semanticMapBuilder = semanticMapBuilder;
    }

    // context: roslyn, build, contextInfo, compilationFlow
    public async Task<SemanticCompilationMap<TSyntaxTreeWrapper>> BuildCompilationMapAsync(IEnumerable<string> codeFiles, string compilationName, CancellationToken cancellationToken)
    {
        var compilationMap = await _semanticMapBuilder.CreateSemanticMapFromFilesAsync(codeFiles, compilationName, cancellationToken);
        _modelStorage.Add(compilationMap);
        return compilationMap;
    }
}