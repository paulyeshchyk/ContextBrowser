using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Assembly;
using RoslynKit.Model.SyntaxWrapper;
using RoslynKit.Syntax;
using RoslynKit.Wrappers;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Phases.Syntax.Parsers;

// context: syntax, build, roslyn
public class CSharpSyntaxParserMethod<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly CSharpSyntaxParserCommentTrivia<TContext> _triviaCommentParser;
    private readonly ContextInfoBuilderDispatcher<TContext> _contextInfoBuilderDispatcher;
    private readonly IRoslynSymbolLoader<MemberDeclarationSyntax, ISymbol> _symbolLoader;

    public CSharpSyntaxParserMethod(
        CSharpSyntaxParserCommentTrivia<TContext> triviaCommentParser,
        ContextInfoBuilderDispatcher<TContext> contextInfoBuilderDispatcher,
        IAppLogger<AppLevel> logger,
        IRoslynSymbolLoader<MemberDeclarationSyntax, ISymbol> symbolLoader)
    {
        _logger = logger;
        _triviaCommentParser = triviaCommentParser;
        _contextInfoBuilderDispatcher = contextInfoBuilderDispatcher;
        _symbolLoader = symbolLoader;
    }

    //context: roslyn, build, ContextInfo
    public async Task ParseMethodSyntaxAsync(MemberDeclarationSyntax availableSyntax, ISemanticModelWrapper semanticModel, TContext typeContext, SemanticOptions options, CancellationToken cancellationToken)
    {
        if (availableSyntax is not TypeDeclarationSyntax typeSyntax)
        {
            _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Warn, $"[{typeContext.Name}]:Syntax is not TypeDeclaration syntax");

            return;
        }

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, "Parsing files: phase 1 - method syntax");

        var methodDeclarationSyntaxies = typeSyntax.FilteredMethodsList(options).ToList();
        if (methodDeclarationSyntaxies.Count == 0)
        {
            _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, $"[{typeContext.Name}]:Syntax has no methods in List");
            return;
        }

        var buildItems = await ParseMethodSyntaxAsync(typeContext, methodDeclarationSyntaxies, semanticModel, cancellationToken).ConfigureAwait(false);
        var extraDomains = new List<string>();
        foreach (var (context, syntax) in buildItems)
        {
            await _triviaCommentParser.ParseAsync(context, syntax, semanticModel, options, cancellationToken).ConfigureAwait(false);
            extraDomains.AddRange(context.Domains);
        }
        typeContext.MergeDomains(extraDomains);
    }

    public async Task<List<(TContext context, MethodDeclarationSyntax syntax)>> ParseMethodSyntaxAsync(TContext parent, IEnumerable<MethodDeclarationSyntax> methods, ISemanticModelWrapper semanticModel, CancellationToken cancellationToken)
    {
        var result = new List<(TContext, MethodDeclarationSyntax)>();
        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, $"Iterating methods [{parent.Name}]", LogLevelNode.Start);

        foreach (var methodSyntax in methods)
        {
            var methodModel = await BuildWrapper(methodSyntax, semanticModel, cancellationToken).ConfigureAwait(false);
            if (methodModel == null)
            {
                _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Warn, $"[{parent.Name}]: не найден символ для метода [{methodSyntax}]");
                continue;
            }

            var context = await _contextInfoBuilderDispatcher.DispatchAndBuildAsync(parent, methodSyntax, semanticModel, cancellationToken).ConfigureAwait(false);
            if (context != null)
            {
                result.Add((context, methodSyntax));
            }
        }

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        return result;
    }

    private async Task<CSharpSyntaxWrapperMethod?> BuildWrapper(MethodDeclarationSyntax methodSyntax, ISemanticModelWrapper semanticModel, CancellationToken cancellationToken)
    {
        var roslynSymbol = await _symbolLoader.LoadSymbolAsync(methodSyntax, semanticModel, cancellationToken).ConfigureAwait(false);
        return roslynSymbol != null
            ? new CSharpSyntaxWrapperMethod(symbol: roslynSymbol, syntax: methodSyntax)
            : null;
    }
}

internal static class TypeDeclarationDyntaxExts
{
    public static IEnumerable<MethodDeclarationSyntax> FilteredMethodsList(this TypeDeclarationSyntax typeSyntax, SemanticOptions options)
    {
        return typeSyntax.DescendantNodes().OfType<MethodDeclarationSyntax>()
            .Where(methodDeclarationSyntax =>
            {
                var mod = methodDeclarationSyntax.GetModifierType();
                return mod.HasValue && options.MethodModifierTypes.Contains(mod.Value);
            })
            .OrderBy(m => m.SpanStart);
    }
}
