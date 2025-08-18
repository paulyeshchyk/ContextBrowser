using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoslynKit.Model;
using RoslynKit.Model.ModelBuilder;
using RoslynKit.Phases;

namespace RoslynKit.Syntax.Parser.Invocation;

// context: csharp, read
public class CSharpInvocationParser<TContext>
    where TContext : ContextInfo, IContextWithReferences<TContext>
{
    private CSharpInvocationReferenceBuilder<TContext> _invocationReferenceBuilder;
    private ISemanticModelBuilder<SyntaxTree, SemanticModel> _semanticModelBuilder;
    private IContextCollector<TContext> _collector;
    private OnWriteLog? _onWriteLog;

    public CSharpInvocationParser(IContextCollector<TContext> collector, IContextFactory<TContext> factory, ISemanticModelBuilder<SyntaxTree, SemanticModel> modelBuilder, CSharpInvocationReferenceBuilder<TContext> invocationReferenceBuilder, RoslynCodeParserOptions options, OnWriteLog? onWriteLog = null) : base()
    {
        _semanticModelBuilder = modelBuilder;
        _collector = collector;
        _onWriteLog = onWriteLog;
        _invocationReferenceBuilder = invocationReferenceBuilder;
    }

    // context: csharp, read
    public void ParseCode(string code, string filePath, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Parsing code: phase 2 - {filePath}");

        // 1. Достаём дерево из файла
        var syntaxTree = CSharpSyntaxTree.ParseText(code, path: filePath, cancellationToken: cancellationToken);

        // 2. Получаем сохранённую модель из хранилища
        var semanticModel = _semanticModelBuilder.ModelStorage.GetModel(syntaxTree);
        if(semanticModel == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"SemanticModel not found for {filePath}");
            return;
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Generating syntax tree: {filePath}");
        // 3. Получаем рут узел
        var root = syntaxTree.GetCompilationUnitRoot(cancellationToken);

        // 4. Обрабатываем все методы, зарегистрированные в коллекторе
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Building method references: {filePath}");
        var theCollection = _collector.GetAll().Where(m => m.ElementType == ContextInfoElementType.method);
        if(!theCollection.Any())
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"No method references found in: {filePath}");
            return;
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Building invocatons", LogLevelNode.Start);
        foreach(var method in theCollection)
        {
            _invocationReferenceBuilder.BuildReferences(method, cancellationToken);
        }
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, string.Empty, LogLevelNode.End);

        _collector.MergeFakeItems();
    }

    public void ParseFiles(string[] filePaths, CancellationToken cancellationToken)
    {
        foreach(var file in filePaths)
        {
            ParseFile(file, cancellationToken);
        }
    }

    // context: csharp, read
    public void ParseFile(string filePath, CancellationToken cancellationToken)
    {
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Cntx, $"Parsing files: phase 2 - {filePath}");

        var code = File.ReadAllText(filePath);
        ParseCode(code, filePath, cancellationToken);
    }
}
