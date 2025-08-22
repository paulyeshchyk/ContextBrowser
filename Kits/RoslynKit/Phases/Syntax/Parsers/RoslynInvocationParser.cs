using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using RoslynKit.Phases.ContextInfoBuilder;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Phases.Syntax.Parsers;

// context: roslyn, read
public class RoslynInvocationParser<TContext> : IInvocationParser
    where TContext : ContextInfo, IContextWithReferences<TContext>
{
    private RoslynInvocationReferenceBuilder<TContext> _invocationReferenceBuilder;
    private IContextCollector<TContext> _collector;
    private OnWriteLog? _onWriteLog;
    private ISemanticModelStorage<ISyntaxTreeWrapper, ISemanticModelWrapper> _treeModelStorage;
    private ISyntaxTreeWrapperBuilder _syntaxTreeWrapperBuilder;

    public RoslynInvocationParser(IContextCollector<TContext> collector, ISemanticModelStorage<ISyntaxTreeWrapper, ISemanticModelWrapper> semanticTreeModelStorage, ISyntaxTreeWrapperBuilder syntaxTreeWrapperBuilder, RoslynInvocationReferenceBuilder<TContext> invocationReferenceBuilder, SemanticOptions options, OnWriteLog? onWriteLog = null) : base()
    {
        _collector = collector;
        _onWriteLog = onWriteLog;
        _invocationReferenceBuilder = invocationReferenceBuilder;
        _syntaxTreeWrapperBuilder = syntaxTreeWrapperBuilder;
        _treeModelStorage = semanticTreeModelStorage;
    }

    // context: roslyn, read
    public void ParseCode(string code, string filePath, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Parsing code: phase 2 - {filePath}");

        // 1. Достаём дерево из файла
        var syntaxTreeWrapper = _syntaxTreeWrapperBuilder.Build(code, filePath, cancellationToken);

        // 2. Получаем сохранённую модель из хранилища
        var semanticModel = _treeModelStorage.GetModel(syntaxTreeWrapper);
        if (semanticModel == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"SemanticModel not found for {filePath}");
            return;
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Generating syntax tree: {filePath}");

        // 3. Получаем рут узел
        var root = syntaxTreeWrapper.GetCompilationUnitRoot(cancellationToken);

        // 4. Обрабатываем все методы, зарегистрированные в коллекторе
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Building method references: {filePath}");
        var theCollection = _collector.GetAll().Where(m => m.ElementType == ContextInfoElementType.method);
        if (!theCollection.Any())
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"No method references found in: {filePath}");
            return;
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Building invocatons", LogLevelNode.Start);
        foreach (var method in theCollection)
        {
            _invocationReferenceBuilder.BuildReferences(method, cancellationToken);
        }
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, string.Empty, LogLevelNode.End);

        _collector.MergeFakeItems();
    }

    public IEnumerable<ContextInfo> ParseFiles(string[] filePaths, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Cntx, "Parsing files: phase 2", LogLevelNode.Start);

        foreach (var file in filePaths)
        {
            ParseFile(file, cancellationToken);
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Cntx, string.Empty, LogLevelNode.End);

        return _collector.GetAll();
    }

    // context: roslyn, read
    public void ParseFile(string filePath, CancellationToken cancellationToken)
    {
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Cntx, $"Parsing files: phase 2 - {filePath}");

        var code = File.ReadAllText(filePath);
        ParseCode(code, filePath, cancellationToken);
    }

    public void RenewContextInfoList(IEnumerable<ContextInfo> contextInfoList)
    {
        _collector.Renew((IEnumerable<TContext>)contextInfoList);
    }
}
