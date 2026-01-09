using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using SemanticKit.Model;
using SemanticKit.Model.Options;
using SemanticKit.Parsers.Strategy.Invocation;

namespace SemanticKit.Parsers.Strategy.Invocation;

// context: roslyn, read
public class InvocationFileParser<TContext, TSyntaxTreeWrapper> : IInvocationFileParser<TContext>
    where TContext : IContextWithReferences<TContext>
    where TSyntaxTreeWrapper : ISyntaxTreeWrapper
{
    private readonly IInvocationBuilder<TContext> _invocationReferenceBuilder;
    private readonly IContextCollector<TContext> _collector;
    private readonly ISemanticModelStorage<TSyntaxTreeWrapper, ISemanticModelWrapper> _treeModelStorage;
    private readonly ISyntaxTreeWrapperBuilder<TSyntaxTreeWrapper> _syntaxTreeWrapperBuilder;
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IInvocationParser<TContext, TSyntaxTreeWrapper> _invocationParser;
    private readonly IAppOptionsStore _optionsStore;


    public InvocationFileParser(
        IContextCollector<TContext> collector,
        ISemanticModelStorage<TSyntaxTreeWrapper,
        ISemanticModelWrapper> semanticTreeModelStorage,
        ISyntaxTreeWrapperBuilder<TSyntaxTreeWrapper> syntaxTreeWrapperBuilder,
        IInvocationBuilder<TContext> invocationReferenceBuilder,
        IAppLogger<AppLevel> logger,
        IAppOptionsStore optionsStore,
        IInvocationParser<TContext, TSyntaxTreeWrapper> invocationParser)
    {
        _collector = collector;
        _logger = logger;
        _invocationReferenceBuilder = invocationReferenceBuilder;
        _syntaxTreeWrapperBuilder = syntaxTreeWrapperBuilder;
        _treeModelStorage = semanticTreeModelStorage;
        _optionsStore = optionsStore;
        _invocationParser = invocationParser;
    }

    // context: roslyn, syntax, read
    public async Task<IEnumerable<TContext>> ParseFilesAsync(string[] filePaths, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var options = _optionsStore.GetOptions<CodeParsingOptions>().SemanticOptions;

        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = options.MaxDegreeOfParallelism,
            CancellationToken = cancellationToken
        };

        _logger.WriteLog(AppLevel.R_Invocation, LogLevel.Cntx, "Parsing files: phase 2", LogLevelNode.Start);

        await Parallel.ForEachAsync(filePaths, parallelOptions, async (filePath, token) =>
        {
            // ParseFileAsync вызывается в пределах установленного лимита ParallelOptions.MaxDegreeOfParallelism
            await ParseFileAsync(filePath, token).ConfigureAwait(false);
        });
        _logger.WriteLog(AppLevel.R_Invocation, LogLevel.Cntx, string.Empty, LogLevelNode.End);

        return _collector.GetAll();
    }

    // context: roslyn, read
    internal async Task ParseFileAsync(string filePath, CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.R_Invocation, LogLevel.Cntx, $"Reading file: {filePath}");
        var code = await System.IO.File.ReadAllTextAsync(filePath, cancellationToken).ConfigureAwait(false);

        _logger.WriteLog(AppLevel.R_Invocation, LogLevel.Cntx, $"Parsing code: {filePath}");
        await _invocationParser.ParseInvocationsAsync(code, filePath, cancellationToken).ConfigureAwait(false);
    }

    // context: roslyn, build
    public void RenewContextInfoList(IEnumerable<TContext> contextInfoList)
    {
        _collector.Renew(contextInfoList);
    }
}