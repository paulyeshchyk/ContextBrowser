using ContextBrowserKit.Log;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

// context: roslyn, build, contextInfo
public class SemanticTreeModelBuilder : ISemanticTreeModelBuilder<ISyntaxTreeWrapper, ISemanticModelWrapper>
{
    private readonly OnWriteLog? _onWriteLog = null;
    private readonly ICompilationBuilder _compilationBuilder;

    public ISemanticModelStorage<ISyntaxTreeWrapper, ISemanticModelWrapper> SemanticTreeModelStorage { get; }

    public ISyntaxTreeWrapperBuilder SyntaxTreeWrapperBuilder { get; }

    public SemanticTreeModelBuilder(ICompilationBuilder compilationBuilder, ISemanticModelStorage<ISyntaxTreeWrapper, ISemanticModelWrapper> modelStorage, ISyntaxTreeWrapperBuilder treeWrapperBuilder, OnWriteLog? onWriteLog = null)
    {
        SemanticTreeModelStorage = modelStorage;
        SyntaxTreeWrapperBuilder = treeWrapperBuilder;
        _onWriteLog = onWriteLog;
        _compilationBuilder = compilationBuilder;
    }

    // context: roslyn, build, contextInfo
    public SemanticCompilationMap BuildCompilationMap(IEnumerable<string> codeFiles, SemanticOptions options, CancellationToken cancellationToken)
    {
        var compilationMap = _compilationBuilder.BuildCompilationMap(codeFiles, cancellationToken);
        SemanticTreeModelStorage.Add(compilationMap);
        return compilationMap;
    }

    // context: roslyn, build, contextInfo
    public SemanticCompilationView BuildCompilationView(string code, string filePath, SemanticOptions options, CancellationToken cancellationToken)
    {
        var syntaxTreeWrapper = SyntaxTreeWrapperBuilder.Build(code, filePath, cancellationToken);
        return BuildCompilationView(syntaxTreeWrapper, options, cancellationToken);
    }

    // context: roslyn, build, contextInfo
    internal SemanticCompilationView BuildCompilationView(ISyntaxTreeWrapper syntaxTreeWrapper, SemanticOptions options, CancellationToken cancellationToken)
    {
        var root = syntaxTreeWrapper.GetCompilationUnitRoot(cancellationToken);

        var model = SemanticTreeModelStorage.GetModel(syntaxTreeWrapper);
        if (model == null)
        {
            // Добавляем текущий syntaxTree во временный список всех деревьев
            var allSyntaxTrees = SemanticTreeModelStorage.GetAllSyntaxTrees().Concat(new[] { syntaxTreeWrapper }).Distinct();

            var compilationWrapper = _compilationBuilder.BuildCompilation(allSyntaxTrees, options.CustomAssembliesPaths, "Parser");
            var themodel = compilationWrapper.GetSemanticModel(syntaxTreeWrapper);

            SemanticTreeModelStorage.Add(syntaxTreeWrapper, themodel);
            return new SemanticCompilationView(themodel, syntaxTreeWrapper, root);
        }
        else
        {
            return new SemanticCompilationView(model, syntaxTreeWrapper, root);
        }
    }
}