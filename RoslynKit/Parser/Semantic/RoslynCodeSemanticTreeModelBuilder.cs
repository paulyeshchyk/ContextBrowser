using LoggerKit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Loader;
using RoslynKit.Model;

namespace RoslynKit.Parser.Semantic;

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
    public Dictionary<SyntaxTree, SemanticModel> BuildModels(IEnumerable<string> codeFiles, RoslynCodeParserOptions options, CancellationToken cancellationToken)
    {
        var compilation = CompilationBuilder.BuildModels(codeFiles, options, _onWriteLog, cancellationToken);
        _modelStorage.AddRange(compilation);
        return compilation;
    }

    // context: csharp, build, contextInfo
    public RoslynCompilationView BuildModel(string code, string filePath, RoslynCodeParserOptions options, CancellationToken cancellationToken)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(text: code, path: filePath, cancellationToken: cancellationToken);
        return BuildModel(syntaxTree, options, cancellationToken);
    }

    // context: csharp, build, contextInfo
    internal RoslynCompilationView BuildModel(SyntaxTree syntaxTree, RoslynCodeParserOptions options, CancellationToken cancellationToken)
    {
        var root = syntaxTree.GetCompilationUnitRoot(cancellationToken);
        return BuildModel(root, syntaxTree, options);
    }

    // context: csharp, build, contextInfo
    internal RoslynCompilationView BuildModel(CompilationUnitSyntax root, SyntaxTree syntaxTree, RoslynCodeParserOptions options)
    {
        var model = _modelStorage.GetModel(syntaxTree);
        if(model == null)
        {
            // Добавляем текущий syntaxTree во временный список всех деревьев
            var allSyntaxTrees = _modelStorage
                .GetAllSyntaxTrees()
                .Concat(new[] { syntaxTree })
                .Distinct();

            var compilation = CompilationBuilder.Build(allSyntaxTrees, options.CustomAssembliesPaths);
            model = compilation.GetSemanticModel(syntaxTree);
            _modelStorage.Add(syntaxTree, model);
        }

        return new RoslynCompilationView(model, syntaxTree, root);
    }
}