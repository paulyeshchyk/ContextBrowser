using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options;
using LoggerKit;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

// context: roslyn, build, contextInfo
public class SemanticTreeModelBuilder<TSyntaxTreeWrapper> : ISemanticTreeModelBuilder<TSyntaxTreeWrapper, ISemanticModelWrapper>
    where TSyntaxTreeWrapper : ISyntaxTreeWrapper
{
    private readonly ICompilationBuilder<TSyntaxTreeWrapper> _compilationBuilder;
    private readonly IAppLogger<AppLevel> _logger;
    private readonly ISemanticModelStorage<TSyntaxTreeWrapper, ISemanticModelWrapper> _modelStorage;
    private readonly ISyntaxTreeWrapperBuilder<TSyntaxTreeWrapper> _treeWrapperBuilder;
    private readonly ISemanticMapExtractor<TSyntaxTreeWrapper> _semanticMapBuilder;

    public SemanticTreeModelBuilder(
        ICompilationBuilder<TSyntaxTreeWrapper> compilationBuilder,
        ISemanticModelStorage<TSyntaxTreeWrapper,
        ISemanticModelWrapper> modelStorage,
        ISyntaxTreeWrapperBuilder<TSyntaxTreeWrapper> treeWrapperBuilder,
        IAppLogger<AppLevel> logger,
        ISemanticMapExtractor<TSyntaxTreeWrapper> semanticMapBuilder)
    {
        _modelStorage = modelStorage;
        _treeWrapperBuilder = treeWrapperBuilder;
        _logger = logger;
        _compilationBuilder = compilationBuilder;
        _semanticMapBuilder = semanticMapBuilder;

    }

    // context: roslyn, build, contextInfo, compilationFlow
    public async Task<SemanticCompilationMap<TSyntaxTreeWrapper>> BuildCompilationMapAsync(IEnumerable<string> codeFiles, SemanticOptions options, CancellationToken cancellationToken)
    {
        var compilationMap = await _semanticMapBuilder.CreateSemanticMapFromFilesAsync(options, codeFiles, cancellationToken);
        _modelStorage.Add(compilationMap);
        return compilationMap;
    }

    // context: roslyn, build, contextInfo
    public async Task<SemanticCompilationView> BuildCompilationViewAsync(string code, string filePath, SemanticOptions options, CancellationToken cancellationToken)
    {
        var syntaxTreeWrapper = await _treeWrapperBuilder.BuildAsync(code, filePath, cancellationToken).ConfigureAwait(false);
        return await BuildCompilationView(syntaxTreeWrapper, options, cancellationToken);
    }

    // context: roslyn, build, contextInfo
    internal async Task<SemanticCompilationView> BuildCompilationView(TSyntaxTreeWrapper syntaxTreeWrapper, SemanticOptions options, CancellationToken cancellationToken)
    {
        var root = syntaxTreeWrapper.GetCompilationUnitRoot(cancellationToken);

        var model = _modelStorage.GetModel(syntaxTreeWrapper);
        if (model == null)
        {
            // Добавляем текущий syntaxTree во временный список всех деревьев
            var allSyntaxTrees = _modelStorage.GetAllSyntaxTrees().Concat([syntaxTreeWrapper]).Distinct();

            var compilationWrapper = await _compilationBuilder.BuildAsync(options, allSyntaxTrees, options.CustomAssembliesPaths, "Parser", cancellationToken).ConfigureAwait(false);
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