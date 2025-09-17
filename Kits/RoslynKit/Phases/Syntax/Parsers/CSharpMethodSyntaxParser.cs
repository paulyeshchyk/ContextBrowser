using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;
using RoslynKit.Phases.ContextInfoBuilder;
using RoslynKit.Wrappers;
using RoslynKit.Wrappers.Syntax;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Phases.Syntax.Parsers;

public class CSharpMethodSyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly CSharpMethodContextInfoBuilder<TContext> _methodContextInfoBuilder;
    private readonly CSharpCommentTriviaSyntaxParser<TContext> _triviaCommentParser;

    public CSharpMethodSyntaxParser(CSharpMethodContextInfoBuilder<TContext> methodContextInfoBuilder, CSharpCommentTriviaSyntaxParser<TContext> triviaCommentParser, IAppLogger<AppLevel> logger)
    {
        _logger = logger;
        _methodContextInfoBuilder = methodContextInfoBuilder;
        _triviaCommentParser = triviaCommentParser;
    }

    //context: roslyn, build
    public void ParseMethodSyntax(MemberDeclarationSyntax availableSyntax, ISemanticModelWrapper semanticModel, TContext typeContext, SemanticOptions options, CancellationToken cancellationToken)
    {
        if (availableSyntax is not TypeDeclarationSyntax typeSyntax)
        {
            _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Warn, $"[{typeContext.Name}]:Syntax is not TypeDeclaration syntax");

            return;
        }

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, $"Parsing files: phase 1 - method syntax");

        var methodDeclarationSyntaxies = typeSyntax.FilteredMethodsList(options);
        if (!methodDeclarationSyntaxies.Any())
        {
            _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, $"[{typeContext.Name}]:Syntax has no methods in List");
            return;
        }

        var buildItems = ParseMethodSyntax(typeContext, methodDeclarationSyntaxies, semanticModel, cancellationToken);

        foreach (var (context, syntax) in buildItems)
        {
            _triviaCommentParser.Parse(context, syntax, semanticModel, options, cancellationToken);
        }
    }

    public List<(TContext context, MethodDeclarationSyntax syntax)> ParseMethodSyntax(TContext parent, IEnumerable<MethodDeclarationSyntax> methods, ISemanticModelWrapper semanticModel, CancellationToken cancellationToken)
    {
        var result = new List<(TContext, MethodDeclarationSyntax)>();
        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, $"Iterating methods [{parent.Name}]", LogLevelNode.Start);

        foreach (var method in methods)
        {
            if (method is not MethodDeclarationSyntax methodDeclaration)
            {
                _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Err, $"[{parent.Name}]: не найден метод");
                continue;
            }
            var methodModel = BuildWrapper(method, semanticModel, cancellationToken);
            if (methodModel == null)
            {
                _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Warn, $"[{parent.Name}]: не найден символ для метода [{method}]");
                continue;
            }

            var context = _methodContextInfoBuilder.BuildContextInfo(parent, method, semanticModel, cancellationToken);
            if (context != null)
            {
                result.Add((context, method));
            }
        }

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        return result;
    }

    private CSharpMethodSyntaxWrapper? BuildWrapper(MethodDeclarationSyntax syntax, ISemanticModelWrapper model, CancellationToken cancellationToken)
    {
        var symbol = CSharpSymbolLoader.LoadSymbol(syntax, model, _logger, cancellationToken);
        if (symbol == null)
        {
            return default;
        }

        return new CSharpMethodSyntaxWrapper(symbol: symbol, syntax: syntax);
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
                return mod.HasValue && (options.MethodModifierTypes?.Contains(mod.Value) ?? false);
            })
            .OrderBy(m => m.SpanStart);
    }
}
