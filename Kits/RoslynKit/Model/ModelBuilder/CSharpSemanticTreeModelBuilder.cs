using ContextBrowserKit.Log;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Model.Storage;
using RoslynKit.Syntax.AssemblyLoader;

namespace RoslynKit.Model.ModelBuilder;

// context: csharp, build, contextInfo
public class CSharpSemanticTreeModelBuilder : ISemanticModelBuilder<SyntaxTree, SemanticModel>
{
    private readonly ISemanticModelStorage<SyntaxTree, SemanticModel> _modelStorage;
    private readonly OnWriteLog? _onWriteLog = null;

    public ISemanticModelStorage<SyntaxTree, SemanticModel> ModelStorage => _modelStorage;

    public CSharpSemanticTreeModelBuilder(ISemanticModelStorage<SyntaxTree, SemanticModel> modelStorage, OnWriteLog? onWriteLog = null)
    {
        _modelStorage = modelStorage;
        _onWriteLog = onWriteLog;
    }

    // context: csharp, build, contextInfo
    public Dictionary<SyntaxTree, SemanticModel> BuildModels(IEnumerable<string> codeFiles, RoslynCodeParserOptions options, CancellationToken cancellationToken)
    {
        var builder = new CSharpCompilationBuilder(options, _onWriteLog);
        var compilation = builder.BuildModels(codeFiles, cancellationToken);
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

            var builder = new CSharpCompilationBuilder(options, _onWriteLog);

            var compilation = builder.Build(allSyntaxTrees, options.CustomAssembliesPaths);
            model = compilation.GetSemanticModel(syntaxTree);
            _modelStorage.Add(syntaxTree, model);
        }

        return new RoslynCompilationView(model, syntaxTree, root);
    }
}