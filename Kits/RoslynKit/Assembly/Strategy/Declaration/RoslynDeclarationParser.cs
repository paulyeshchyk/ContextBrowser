using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using RoslynKit;
using RoslynKit.Assembly;
using RoslynKit.Assembly.Strategy.Declaration;
using RoslynKit.Model.Meta;
using SemanticKit;
using SemanticKit.Model;
using SemanticKit.Model.Options;
using SemanticKit.Parsers.Strategy.Declaration;

namespace RoslynKit.Assembly.Strategy.Declaration;

// context: roslyn, read
public class RoslynDeclarationParser<TContext> : IDeclarationParser<TContext, RoslynSyntaxTreeWrapper>
    where TContext : IContextWithReferences<TContext>
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IAppOptionsStore _optionsStore;

    public RoslynDeclarationParser(IAppLogger<AppLevel> logger, IAppOptionsStore optionsStore)
    {
        _logger = logger;
        _optionsStore = optionsStore;
    }

    // context: roslyn, read

    public async Task ParseAsync(ISemanticSyntaxRouter<TContext> router, CompilationMap<RoslynSyntaxTreeWrapper> mapItem, CancellationToken cancellationToken)
    {
        var tree = mapItem.SyntaxTree;
        var model = mapItem.SemanticModel;

        var options = _optionsStore.GetOptions<CodeParsingOptions>().SemanticOptions;

        cancellationToken.ThrowIfCancellationRequested();
        var availableSyntaxies = tree.GetAvailableSyntaxies(options, cancellationToken).ToList();

        if (availableSyntaxies.Count == 0)
        {
            _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, $"Model has no members: {tree.FilePath}");
            return;
        }

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Cntx, $"Phase 1: Parsing declarations - {tree.FilePath}", LogLevelNode.Start);
        await router.RouteAsync(availableSyntaxies, model, options, cancellationToken);
        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Cntx, string.Empty, LogLevelNode.End);

    }
}
