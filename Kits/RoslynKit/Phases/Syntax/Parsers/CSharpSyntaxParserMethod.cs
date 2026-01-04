using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.AWrappers;
using RoslynKit.Model.SyntaxWrapper;
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

    public CSharpSyntaxParserMethod(
        CSharpSyntaxParserCommentTrivia<TContext> triviaCommentParser,
        ContextInfoBuilderDispatcher<TContext> contextInfoBuilderDispatcher,
        IAppLogger<AppLevel> logger)
    {
        _logger = logger;
        _triviaCommentParser = triviaCommentParser;
        _contextInfoBuilderDispatcher = contextInfoBuilderDispatcher;
    }

    //context: roslyn, build, ContextInfo
    public void ParseMethodSyntax(MemberDeclarationSyntax availableSyntax, ISemanticModelWrapper semanticModel, TContext typeContext, SemanticOptions options, CancellationToken cancellationToken)
    {
        if (availableSyntax is not TypeDeclarationSyntax typeSyntax)
        {
            _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Warn, $"[{typeContext.Name}]:Syntax is not TypeDeclaration syntax");

            return;
        }

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, "Parsing files: phase 1 - method syntax");

        var methodDeclarationSyntaxies = typeSyntax.FilteredMethodsList(options).ToList();
        if (!methodDeclarationSyntaxies.Any())
        {
            _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, $"[{typeContext.Name}]:Syntax has no methods in List");
            return;
        }

        var buildItems = ParseMethodSyntax(typeContext, methodDeclarationSyntaxies, semanticModel, cancellationToken);
        var extraDomains = new List<string>();
        foreach (var (context, syntax) in buildItems)
        {
            _triviaCommentParser.Parse(context, syntax, semanticModel, options, cancellationToken);
            extraDomains.AddRange(context.Domains);
        }
        typeContext.MergeDomains(extraDomains);
    }

    public List<(TContext context, MethodDeclarationSyntax syntax)> ParseMethodSyntax(TContext parent, IEnumerable<MethodDeclarationSyntax> methods, ISemanticModelWrapper semanticModel, CancellationToken cancellationToken)
    {
        var result = new List<(TContext, MethodDeclarationSyntax)>();
        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, $"Iterating methods [{parent.Name}]", LogLevelNode.Start);

        foreach (var methodSyntax in methods)
        {
            var methodModel = BuildWrapper(methodSyntax, semanticModel, cancellationToken);
            if (methodModel == null)
            {
                _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Warn, $"[{parent.Name}]: не найден символ для метода [{methodSyntax}]");
                continue;
            }

            var context = _contextInfoBuilderDispatcher.DispatchAndBuild(parent, methodSyntax, semanticModel, cancellationToken);
            if (context != null)
            {
                result.Add((context, methodSyntax));
            }
        }

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        return result;
    }

    private CSharpSyntaxWrapperMethod? BuildWrapper(MethodDeclarationSyntax methodSyntax, ISemanticModelWrapper semanticModel, CancellationToken cancellationToken)
    {
        var roslynSymbol = CSharpSymbolLoader.LoadSymbol(methodSyntax, semanticModel, _logger, cancellationToken);
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
