using ContextBrowserKit.Log;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Route.Tree;

// context: roslyn, build, contextInfo
public class BaseTreeModelBuilder : ISemanticModelBuilder<ISyntaxTreeWrapper, ISemanticModelWrapper>
{
    private readonly ISemanticModelStorage<ISyntaxTreeWrapper, ISemanticModelWrapper> _modelStorage;
    private readonly OnWriteLog? _onWriteLog = null;
    private readonly ISyntaxTreeWrapperBuilder _treeWrapperBuilder;
    private readonly ICompilationBuilder _compilationBuilder;

    public ISemanticModelStorage<ISyntaxTreeWrapper, ISemanticModelWrapper> ModelStorage => _modelStorage;

    public BaseTreeModelBuilder(ICompilationBuilder compilationBuilder, ISemanticModelStorage<ISyntaxTreeWrapper, ISemanticModelWrapper> modelStorage, ISyntaxTreeWrapperBuilder treeWrapperBuilder, OnWriteLog? onWriteLog = null)
    {
        _modelStorage = modelStorage;
        _onWriteLog = onWriteLog;
        _treeWrapperBuilder = treeWrapperBuilder;
        _compilationBuilder = compilationBuilder;
    }

    // context: roslyn, build, contextInfo
    public Dictionary<ISyntaxTreeWrapper, ISemanticModelWrapper> BuildModels(IEnumerable<string> codeFiles, SemanticOptions options, CancellationToken cancellationToken)
    {
        var compilation = _compilationBuilder.BuildModels(codeFiles, cancellationToken);
        _modelStorage.AddRange(compilation);
        return compilation;
    }

    // context: roslyn, build, contextInfo
    public RoslynCompilationView BuildModel(string code, string filePath, SemanticOptions options, CancellationToken cancellationToken)
    {
        var wrapper = _treeWrapperBuilder.BuildTree(code, filePath, cancellationToken);
        return BuildModel(wrapper, options, cancellationToken);
    }

    // context: roslyn, build, contextInfo
    internal RoslynCompilationView BuildModel(ISyntaxTreeWrapper syntaxTree, SemanticOptions options, CancellationToken cancellationToken)
    {
        var root = syntaxTree.GetCompilationUnitRoot(cancellationToken);

        var model = _modelStorage.GetModel(syntaxTree);
        if (model == null)
        {
            // Добавляем текущий syntaxTree во временный список всех деревьев
            var allSyntaxTrees = _modelStorage.GetAllSyntaxTrees().Concat(new[] { syntaxTree }).Distinct();

            var compilationWrapper = _compilationBuilder.BuildCompilation(allSyntaxTrees, options.CustomAssembliesPaths, "Parser");
            var themodel = compilationWrapper.GetSemanticModel(syntaxTree);
            _modelStorage.Add(syntaxTree, themodel);
            return new RoslynCompilationView(themodel, syntaxTree, root);
        }
        else
        {
            return new RoslynCompilationView(model, syntaxTree, root);
        }
    }
}