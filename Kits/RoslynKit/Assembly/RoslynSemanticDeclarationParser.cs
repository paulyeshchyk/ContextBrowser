using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using RoslynKit.Assembly;
using RoslynKit.Model.Meta;
using SemanticKit;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Assembly;

// context: roslyn, read
public class RoslynSemanticDeclarationParser<TContext> : ISemanticDeclarationParser<TContext, RoslynSyntaxTreeWrapper>
    where TContext : IContextWithReferences<TContext>
{
    private readonly IAppLogger<AppLevel> _logger;

    public RoslynSemanticDeclarationParser(IAppLogger<AppLevel> logger) => _logger = logger;

    // context: roslyn, read
    public Task Parse(ISemanticSyntaxRouter<TContext> router, SemanticOptions options, CompilationMap<RoslynSyntaxTreeWrapper> mapItem, CancellationToken cancellationToken)
    {
        var tree = mapItem.SyntaxTree;
        var model = mapItem.SemanticModel;

        cancellationToken.ThrowIfCancellationRequested();

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, $"Phase 1: Parsing declarations - {tree.FilePath}", LogLevelNode.Start);

        var availableSyntaxies = tree.GetAvailableSyntaxies(options, cancellationToken).ToList();

        if (availableSyntaxies.Count != 0)
        {
            router.Route(availableSyntaxies, model, options, cancellationToken);
        }
        else
        {
            _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, $"Model has no members: {tree.FilePath}");
        }

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        return Task.CompletedTask;
    }
}
