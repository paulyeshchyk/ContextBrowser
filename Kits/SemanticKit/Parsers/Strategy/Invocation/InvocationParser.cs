using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace SemanticKit.Parsers.Strategy.Invocation;

public class InvocationParser<TContext, TSyntaxTreeWrapper> : IInvocationParser<TContext, TSyntaxTreeWrapper>
    where TContext : IContextWithReferences<TContext>
    where TSyntaxTreeWrapper : ISyntaxTreeWrapper
{
    private readonly ISyntaxTreeWrapperBuilder<TSyntaxTreeWrapper> _syntaxTreeWrapperBuilder;
    private readonly IContextCollector<TContext> _collector;
    private readonly IAppLogger<AppLevel> _logger;
    private readonly ISemanticModelStorage<TSyntaxTreeWrapper, ISemanticModelWrapper> _treeModelStorage;
    private readonly IInvocationBuilder<TContext> _invocationReferenceBuilder;
    private readonly IAppOptionsStore _optionsStore;

    public InvocationParser(
        ISyntaxTreeWrapperBuilder<TSyntaxTreeWrapper> syntaxTreeWrapperBuilder,
        IContextCollector<TContext> collector,
        IAppLogger<AppLevel> logger,
        ISemanticModelStorage<TSyntaxTreeWrapper, ISemanticModelWrapper> treeModelStorage,
        IInvocationBuilder<TContext> invocationReferenceBuilder,
        IAppOptionsStore optionsStore)
    {
        _syntaxTreeWrapperBuilder = syntaxTreeWrapperBuilder;
        _collector = collector;
        _logger = logger;
        _treeModelStorage = treeModelStorage;
        _invocationReferenceBuilder = invocationReferenceBuilder;
        _optionsStore = optionsStore;
    }

    // context: roslyn, read

    public async Task<bool> ParseInvocationsAsync(string code, string filePath, CancellationToken cancellationToken)
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
        await BuildInvocationsAsync(filePath, theCollection.ToList(), cancellationToken).ConfigureAwait(false);

        _collector.MergeFakeItems();

        return true;
    }

    // context: roslyn, syntax, read
    internal async Task BuildInvocationsAsync(string filePath, IEnumerable<TContext> theCollection, CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.R_Invocation, LogLevel.Dbg, $"Building invocatons {filePath}", LogLevelNode.Start);
        var semanticOptions = _optionsStore.GetOptions<CodeParsingOptions>().SemanticOptions;

        var tasks = theCollection.Select(async method => await _invocationReferenceBuilder.BuildReferencesAsync(method, semanticOptions, cancellationToken).ConfigureAwait(false));
        await Task.WhenAll(tasks);
        _logger.WriteLog(AppLevel.R_Invocation, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }
}