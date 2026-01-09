using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options;
using LoggerKit;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

// context: roslyn, build, contextInfo
public class SemanticCompilationViewBuilder<TSyntaxTreeWrapper> : ISemanticCompilationViewBuilder<TSyntaxTreeWrapper, ISemanticModelWrapper>
    where TSyntaxTreeWrapper : ISyntaxTreeWrapper
{
    private readonly ICompilationBuilder<TSyntaxTreeWrapper> _compilationBuilder;
    private readonly ISemanticModelStorage<TSyntaxTreeWrapper, ISemanticModelWrapper> _modelStorage;
    private readonly ISyntaxTreeWrapperBuilder<TSyntaxTreeWrapper> _treeWrapperBuilder;

    public SemanticCompilationViewBuilder(
        ICompilationBuilder<TSyntaxTreeWrapper> compilationBuilder,
        ISemanticModelStorage<TSyntaxTreeWrapper,
        ISemanticModelWrapper> modelStorage,
        ISyntaxTreeWrapperBuilder<TSyntaxTreeWrapper> treeWrapperBuilder)
    {
        _modelStorage = modelStorage;
        _treeWrapperBuilder = treeWrapperBuilder;
        _compilationBuilder = compilationBuilder;
    }

    // context: roslyn, build, contextInfo
    public async Task<SemanticCompilationView> BuildCompilationViewAsync(string code, string filePath, string compilationName, CancellationToken cancellationToken)
    {
        var syntaxTreeWrapper = await _treeWrapperBuilder.BuildAsync(code, filePath, cancellationToken).ConfigureAwait(false);
        return await BuildCompilationView(syntaxTreeWrapper, compilationName, cancellationToken);
    }

    // context: roslyn, build, contextInfo
    internal async Task<SemanticCompilationView> BuildCompilationView(TSyntaxTreeWrapper syntaxTreeWrapper, string compilationName, CancellationToken cancellationToken)
    {
        var root = syntaxTreeWrapper.GetCompilationUnitRoot(cancellationToken);

        var model = _modelStorage.GetModel(syntaxTreeWrapper)
            ?? await BuildModelWrapper(syntaxTreeWrapper, compilationName, cancellationToken).ConfigureAwait(false);
        return new SemanticCompilationView(model, syntaxTreeWrapper, root);
    }

    private async Task<ISemanticModelWrapper> BuildModelWrapper(TSyntaxTreeWrapper syntaxTreeWrapper, string compilationName, CancellationToken cancellationToken)
    {
        // Добавляем текущий syntaxTree во временный список всех деревьев
        var allSyntaxTrees = _modelStorage.GetAllSyntaxTrees().Concat([syntaxTreeWrapper]).Distinct();

        var compilationWrapper = await _compilationBuilder.BuildAsync(allSyntaxTrees, compilationName, cancellationToken).ConfigureAwait(false);
        var themodel = compilationWrapper.GetSemanticModel(syntaxTreeWrapper);

        _modelStorage.Add(syntaxTreeWrapper, themodel);
        return themodel;
    }
}