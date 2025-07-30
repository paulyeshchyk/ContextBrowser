using ContextBrowser.LoggerKit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ContextBrowser.SourceKit.Roslyn;

// context: csharp, build, contextInfo
public class RoslynCodeSemanticTreeModelBuilder : ISemanticModelBuilder
{
    private readonly ISemanticModelStorage _modelStorage;
    private readonly OnWriteLog? _onWriteLog = null;

    public ISemanticModelStorage ModelStorage => _modelStorage;

    public RoslynCodeSemanticTreeModelBuilder(ISemanticModelStorage modelStorage, OnWriteLog? onWriteLog = null)
    {
        _modelStorage = modelStorage;
        _onWriteLog = onWriteLog;
    }

    // context: csharp, build, contextInfo
    public Dictionary<SyntaxTree, SemanticModel> BuildModels(IEnumerable<string> codeFiles, RoslynCodeParserOptions options)
    {
        var result = RoslynCodeCompilationBuilder.BuildModels(codeFiles, options, _onWriteLog);
        _modelStorage.AddRange(result);
        return result;
    }

    // context: csharp, build, contextInfo
    public RoslynCompilationView BuildModel(string code, string filePath, RoslynCodeParserOptions options)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(text: code, path: filePath, cancellationToken: CancellationToken.None);
        return BuildModel(syntaxTree, options);
    }

    private RoslynCompilationView BuildModel(SyntaxTree syntaxTree, RoslynCodeParserOptions options)
    {
        var root = syntaxTree.GetCompilationUnitRoot(CancellationToken.None);
        return BuildModel(root, syntaxTree, options);
    }

    private RoslynCompilationView BuildModel(CompilationUnitSyntax root, SyntaxTree syntaxTree, RoslynCodeParserOptions options)
    {
        var model = _modelStorage.GetModel(syntaxTree);
        if(model == null)
        {
            // Добавляем текущий syntaxTree во временный список всех деревьев
            var allSyntaxTrees = _modelStorage
                .GetAllSyntaxTrees()
                .Concat(new[] { syntaxTree })
                .Distinct();

            var compilation = RoslynCodeCompilationBuilder.Build(allSyntaxTrees, options.CustomAssembliesPaths);
            model = compilation.GetSemanticModel(syntaxTree);
            _modelStorage.Add(syntaxTree, model);
        }

        return new RoslynCompilationView(model, syntaxTree, root);
    }
}
