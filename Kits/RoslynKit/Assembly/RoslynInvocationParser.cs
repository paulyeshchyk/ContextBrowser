using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using RoslynKit;
using RoslynKit.Assembly;
using RoslynKit.ContextInfoBuilder;
using RoslynKit.Model.Meta;
using RoslynKit.Phases;
using RoslynKit.Phases.ContextInfoBuilder;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Assembly;

// context: roslyn, read
public class RoslynInvocationParser<TContext, TSyntaxTreeWrapper> : IInvocationParser<TContext>
    where TContext : IContextWithReferences<TContext>
    where TSyntaxTreeWrapper : ISyntaxTreeWrapper
{
    private readonly SemanticInvocationReferenceBuilder<TContext> _invocationReferenceBuilder;
    private readonly IContextCollector<TContext> _collector;
    private readonly ISemanticModelStorage<TSyntaxTreeWrapper, ISemanticModelWrapper> _treeModelStorage;
    private readonly ISyntaxTreeWrapperBuilder<TSyntaxTreeWrapper> _syntaxTreeWrapperBuilder;
    private readonly IAppLogger<AppLevel> _logger;

    public RoslynInvocationParser(
        IContextCollector<TContext> collector,
        ISemanticModelStorage<TSyntaxTreeWrapper,
        ISemanticModelWrapper> semanticTreeModelStorage,
        ISyntaxTreeWrapperBuilder<TSyntaxTreeWrapper> syntaxTreeWrapperBuilder,
        SemanticInvocationReferenceBuilder<TContext> invocationReferenceBuilder,
        IAppLogger<AppLevel> logger)
    {
        _collector = collector;
        _logger = logger;
        _invocationReferenceBuilder = invocationReferenceBuilder;
        _syntaxTreeWrapperBuilder = syntaxTreeWrapperBuilder;
        _treeModelStorage = semanticTreeModelStorage;
    }

    // context: roslyn, syntax, read
    public async Task<IEnumerable<TContext>> ParseFilesAsync(string[] filePaths, SemanticOptions options, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = options.MaxDegreeOfParallelism,
            CancellationToken = cancellationToken
        };

        _logger.WriteLog(AppLevel.R_Invocation, LogLevel.Cntx, "Parsing files: phase 2", LogLevelNode.Start);

        await Parallel.ForEachAsync(filePaths, parallelOptions, async (filePath, token) =>
        {
            // ParseFileAsync вызывается в пределах установленного лимита
            await ParseFileAsync(filePath, options, token);
        });
        _logger.WriteLog(AppLevel.R_Invocation, LogLevel.Cntx, string.Empty, LogLevelNode.End);

        return _collector.GetAll();
    }

    // context: roslyn, read
    public async Task ParseFileAsync(string filePath, SemanticOptions options, CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.R_Invocation, LogLevel.Cntx, $"Reading file: {filePath}");
        var code = await File.ReadAllTextAsync(filePath, cancellationToken);

        _logger.WriteLog(AppLevel.R_Invocation, LogLevel.Cntx, $"Parsing code: {filePath}");
        await ParseCodeAsync(code, filePath, options, cancellationToken);
    }

    // context: roslyn, read
    public async Task<bool> ParseCodeAsync(string code, string filePath, SemanticOptions options, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // 1. Достаём дерево из файла
        var syntaxTreeWrapper = await _syntaxTreeWrapperBuilder.BuildAsync(code, filePath, cancellationToken).ConfigureAwait(false);

        // 2. Получаем сохранённую модель из хранилища
        var semanticModel = _treeModelStorage.GetModel(syntaxTreeWrapper);
        if (semanticModel == null)
        {
            _logger.WriteLog(AppLevel.R_Invocation, LogLevel.Err, $"[FAIL] SemanticModel not found for {filePath}");
            return false;
        }

        // 3. Обрабатываем все методы, зарегистрированные в коллекторе
        _logger.WriteLog(AppLevel.R_Invocation, LogLevel.Dbg, $"Building method references: {filePath}");
        var theCollection = _collector.GetAll().Where(m => m.ElementType == ContextInfoElementType.method).ToList();
        if (theCollection.Count == 0)
        {
            _logger.WriteLog(AppLevel.R_Invocation, LogLevel.Err, $"[FAIL] No method references found in: {filePath}");
            return false;
        }

        // 4. Строим все связи
        await BuildReferencesAsync(filePath, theCollection.ToList(), options, cancellationToken).ConfigureAwait(false);

        Flush();

        return true;
    }

    // context: roslyn, syntax, read
    internal async Task BuildReferencesAsync(string filePath, IEnumerable<TContext> theCollection, SemanticOptions options, CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.R_Invocation, LogLevel.Dbg, $"Building invocatons {filePath}", LogLevelNode.Start);
        var tasks = theCollection.Select(method => _invocationReferenceBuilder.BuildReferencesAsync(method, options, cancellationToken));
        await Task.WhenAll(tasks);
        _logger.WriteLog(AppLevel.R_Invocation, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }

    // context: roslyn, build
    public void RenewContextInfoList(IEnumerable<TContext> contextInfoList)
    {
        _collector.Renew(contextInfoList);
    }

    public void Flush()
    {
        _collector.MergeFakeItems();
    }
}
