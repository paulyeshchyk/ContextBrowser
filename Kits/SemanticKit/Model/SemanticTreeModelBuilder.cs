using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ContextBrowserKit.Options;
using LoggerKit;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

// context: roslyn, build, contextInfo
public class SemanticTreeModelBuilder : ISemanticTreeModelBuilder<ISyntaxTreeWrapper, ISemanticModelWrapper>
{
    private readonly ICompilationBuilder _compilationBuilder;
    private readonly IAppLogger<AppLevel> _logger;
    private readonly ISemanticModelStorage<ISyntaxTreeWrapper, ISemanticModelWrapper> _modelStorage;
    private readonly ISyntaxTreeWrapperBuilder _treeWrapperBuilder;

    public SemanticTreeModelBuilder(ICompilationBuilder compilationBuilder, ISemanticModelStorage<ISyntaxTreeWrapper, ISemanticModelWrapper> modelStorage, ISyntaxTreeWrapperBuilder treeWrapperBuilder, IAppLogger<AppLevel> logger)
    {
        _modelStorage = modelStorage;
        _treeWrapperBuilder = treeWrapperBuilder;
        _logger = logger;
        _compilationBuilder = compilationBuilder;
    }

    // context: roslyn, build, contextInfo
    public SemanticCompilationMap BuildCompilationMap(IEnumerable<string> codeFiles, SemanticOptions options, CancellationToken cancellationToken)
    {
        var compilationMap = _compilationBuilder.BuildCompilationMap(options, codeFiles, cancellationToken);
        _modelStorage.Add(compilationMap);
        return compilationMap;
    }

    // context: roslyn, build, contextInfo
    public SemanticCompilationView BuildCompilationView(string code, string filePath, SemanticOptions options, CancellationToken cancellationToken)
    {
        var syntaxTreeWrapper = _treeWrapperBuilder.Build(code, filePath, cancellationToken);
        return BuildCompilationView(syntaxTreeWrapper, options, cancellationToken);
    }

    // context: roslyn, build, contextInfo
    internal SemanticCompilationView BuildCompilationView(ISyntaxTreeWrapper syntaxTreeWrapper, SemanticOptions options, CancellationToken cancellationToken)
    {
        var root = syntaxTreeWrapper.GetCompilationUnitRoot(cancellationToken);

        var model = _modelStorage.GetModel(syntaxTreeWrapper);
        if (model == null)
        {
#warning Distinct?????
            // Добавляем текущий syntaxTree во временный список всех деревьев
            var allSyntaxTrees = _modelStorage.GetAllSyntaxTrees().Concat([syntaxTreeWrapper]).Distinct();

            var compilationWrapper = _compilationBuilder.BuildCompilation(options, allSyntaxTrees, options.CustomAssembliesPaths, "Parser");
            var themodel = compilationWrapper.GetSemanticModel(syntaxTreeWrapper);

            _modelStorage.Add(syntaxTreeWrapper, themodel);
            return new SemanticCompilationView(themodel, syntaxTreeWrapper, root);
        }
        else
        {
            return new SemanticCompilationView(model, syntaxTreeWrapper, root);
        }
    }
}