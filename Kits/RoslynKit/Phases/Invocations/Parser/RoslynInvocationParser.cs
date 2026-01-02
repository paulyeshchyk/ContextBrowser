using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using RoslynKit.Phases.ContextInfoBuilder;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Phases.Invocations.Parser;

// context: roslyn, read
public class RoslynInvocationParser<TContext> : IInvocationParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly SemanticInvocationReferenceBuilder<TContext> _invocationReferenceBuilder;
    private readonly IContextCollector<TContext> _collector;
    private readonly ISemanticModelStorage<ISyntaxTreeWrapper, ISemanticModelWrapper> _treeModelStorage;
    private readonly ISyntaxTreeWrapperBuilder _syntaxTreeWrapperBuilder;
    private readonly IAppLogger<AppLevel> _logger;

    public RoslynInvocationParser(
        IContextCollector<TContext> collector,
        ISemanticModelStorage<ISyntaxTreeWrapper,
        ISemanticModelWrapper> semanticTreeModelStorage,
        ISyntaxTreeWrapperBuilder syntaxTreeWrapperBuilder,
        SemanticInvocationReferenceBuilder<TContext> invocationReferenceBuilder,
        IAppLogger<AppLevel> logger)
    {
        _collector = collector;
        _logger = logger;
        _invocationReferenceBuilder = invocationReferenceBuilder;
        _syntaxTreeWrapperBuilder = syntaxTreeWrapperBuilder;
        _treeModelStorage = semanticTreeModelStorage;
    }

    // context: roslyn, read
    public void ParseCode(string code, string filePath, SemanticOptions options, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.WriteLog(AppLevel.R_Invocation, LogLevel.Dbg, $"Parsing code: phase 2 - {filePath}");

        // 1. Достаём дерево из файла
        var syntaxTreeWrapper = _syntaxTreeWrapperBuilder.Build(code, filePath, cancellationToken);

        // 2. Получаем сохранённую модель из хранилища
        var semanticModel = _treeModelStorage.GetModel(syntaxTreeWrapper);
        if (semanticModel == null)
        {
            _logger.WriteLog(AppLevel.R_Invocation, LogLevel.Warn, $"[FAIL] SemanticModel not found for {filePath}");
            return;
        }

        // 3. Обрабатываем все методы, зарегистрированные в коллекторе
        _logger.WriteLog(AppLevel.R_Invocation, LogLevel.Dbg, $"Building method references: {filePath}");
        var theCollection = _collector.GetAll().Where(m => m.ElementType == ContextInfoElementType.method).ToList();
        if (!theCollection.Any())
        {
            _logger.WriteLog(AppLevel.R_Invocation, LogLevel.Err, $"[FAIL] No method references found in: {filePath}");
            return;
        }

        // 4. Строим все связи
        BuildReferences(filePath, theCollection.ToList(), options, cancellationToken);

        _collector.MergeFakeItems();
    }

    private void BuildReferences(string filePath, IEnumerable<TContext> theCollection, SemanticOptions options, CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.R_Invocation, LogLevel.Dbg, $"Building invocatons {filePath}", LogLevelNode.Start);
        foreach (var method in theCollection)
        {
            _invocationReferenceBuilder.BuildReferences(method, options, cancellationToken);
        }
        _logger.WriteLog(AppLevel.R_Invocation, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }

    public IEnumerable<TContext> ParseFiles(string[] filePaths, SemanticOptions options, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _logger.WriteLog(AppLevel.R_Invocation, LogLevel.Cntx, "Parsing files: phase 2", LogLevelNode.Start);

        foreach (var file in filePaths)
        {
            ParseFile(file, options, cancellationToken);
        }

        _logger.WriteLog(AppLevel.R_Invocation, LogLevel.Cntx, string.Empty, LogLevelNode.End);

        return _collector.GetAll();
    }

    // context: roslyn, read
    public void ParseFile(string filePath, SemanticOptions options, CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.R_Invocation, LogLevel.Cntx, $"Parsing file: phase 2 - {filePath}");

        var code = File.ReadAllText(filePath);
        ParseCode(code, filePath, options, cancellationToken);
    }

    public void RenewContextInfoList(IEnumerable<TContext> contextInfoList)
    {
        _collector.Renew(contextInfoList);
    }
}
