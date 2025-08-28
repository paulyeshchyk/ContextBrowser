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
    private readonly RoslynInvocationReferenceBuilder<TContext> _invocationReferenceBuilder;
    private readonly IContextCollector<TContext> _collector;
    private readonly OnWriteLog? _onWriteLog;
    private readonly ISemanticModelStorage<ISyntaxTreeWrapper, ISemanticModelWrapper> _treeModelStorage;
    private readonly ISyntaxTreeWrapperBuilder _syntaxTreeWrapperBuilder;

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
        _onWriteLog?.Invoke(AppLevel.R_Inv, LogLevel.Dbg, $"Parsing code: phase 2 - {filePath}");

        // 1. Достаём дерево из файла
        var syntaxTreeWrapper = _syntaxTreeWrapperBuilder.Build(code, filePath, cancellationToken);

        // 2. Получаем сохранённую модель из хранилища
        var semanticModel = _treeModelStorage.GetModel(syntaxTreeWrapper);
        if (semanticModel == null)
        {
            _onWriteLog?.Invoke(AppLevel.R_Inv, LogLevel.Warn, $"[FAIL] SemanticModel not found for {filePath}");
            return;
        }

        _onWriteLog?.Invoke(AppLevel.R_Inv, LogLevel.Dbg, $"Generating syntax tree: {filePath}");

        // 3. Получаем рут узел
        var root = syntaxTreeWrapper.GetCompilationUnitRoot(cancellationToken);

        // 4. Обрабатываем все методы, зарегистрированные в коллекторе
        _onWriteLog?.Invoke(AppLevel.R_Inv, LogLevel.Dbg, $"Building method references: {filePath}");
        var theCollection = _collector.GetAll().Where(m => m.ElementType == ContextInfoElementType.method);
        if (!theCollection.Any())
        {
            _onWriteLog?.Invoke(AppLevel.R_Inv, LogLevel.Err, $"[FAIL] No method references found in: {filePath}");
            return;
        }

        // 4. Строим все связи
        BuildReferences(filePath, theCollection, cancellationToken);

        _collector.MergeFakeItems();
    }

    private void BuildReferences(string filePath, IEnumerable<TContext> theCollection, CancellationToken cancellationToken)
    {
        _onWriteLog?.Invoke(AppLevel.R_Inv, LogLevel.Dbg, $"Building invocatons {filePath}", LogLevelNode.Start);
        foreach (var method in theCollection)
        {
            _invocationReferenceBuilder.BuildReferences(method, cancellationToken);
        }
        _onWriteLog?.Invoke(AppLevel.R_Inv, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }

    public IEnumerable<ContextInfo> ParseFiles(string[] filePaths, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _onWriteLog?.Invoke(AppLevel.R_Inv, LogLevel.Cntx, "Parsing files: phase 2", LogLevelNode.Start);

        foreach (var file in filePaths)
        {
            ParseFile(file, cancellationToken);
        }

        _onWriteLog?.Invoke(AppLevel.R_Inv, LogLevel.Cntx, string.Empty, LogLevelNode.End);

        return _collector.GetAll();
    }

    // context: roslyn, read
    public void ParseFile(string filePath, CancellationToken cancellationToken)
    {
        _onWriteLog?.Invoke(AppLevel.R_Inv, LogLevel.Cntx, $"Parsing files: phase 2 - {filePath}");

        var code = File.ReadAllText(filePath);
        ParseCode(code, filePath, cancellationToken);
    }

    public void RenewContextInfoList(IEnumerable<ContextInfo> contextInfoList)
    {
        _collector.Renew((IEnumerable<TContext>)contextInfoList);
    }
}
