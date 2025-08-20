using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;
using RoslynKit.Route.Phases.ContextInfoBuilder;
using RoslynKit.Route.Wrappers;
using RoslynKit.Route.Wrappers.Syntax;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Route.Phases.Syntax.Parsers;

public class CSharpMethodSyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly SemanticOptions _options;
    private readonly OnWriteLog? _onWriteLog;
    private readonly CSharpMethodContextInfoBuilder<TContext> _methodContextInfoBuilder;
    private readonly CSharpCommentTriviaSyntaxParser<TContext> _triviaCommentParser;

    public CSharpMethodSyntaxParser(CSharpMethodContextInfoBuilder<TContext> methodContextInfoBuilder, CSharpCommentTriviaSyntaxParser<TContext> triviaCommentParser, SemanticOptions options, OnWriteLog? onWriteLog)
    {
        _options = options;
        _onWriteLog = onWriteLog;
        _methodContextInfoBuilder = methodContextInfoBuilder;
        _triviaCommentParser = triviaCommentParser;
    }

    //context: csharp, build
    public void ParseMethodSyntax(MemberDeclarationSyntax availableSyntax, ISemanticModelWrapper semanticModel, TContext typeContext)
    {
        if(availableSyntax is not TypeDeclarationSyntax typeSyntax)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"[{typeContext.Name}]:Syntax is not TypeDeclaration syntax");

            return;
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Parsing files: phase 1 - method syntax");

        var methodDeclarationSyntaxies = typeSyntax.FilteredMethodsList(_options);
        if(!methodDeclarationSyntaxies.Any())
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[{typeContext.Name}]:Syntax has no methods in List");
            return;
        }

        var buildItems = ParseMethodSyntax(typeContext, methodDeclarationSyntaxies, semanticModel);

        foreach(var item in buildItems)
        {
            _triviaCommentParser.Parse(item.Item1, item.Item2, semanticModel);
        }
    }


    public List<(TContext context, MethodDeclarationSyntax syntax)> ParseMethodSyntax(TContext parent, IEnumerable<MethodDeclarationSyntax> methods, ISemanticModelWrapper semanticModel)
    {
        var result = new List<(TContext, MethodDeclarationSyntax)>();
        _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Dbg, $"Iterating methods [{parent.Name}]", LogLevelNode.Start);

        foreach(var method in methods)
        {
            if(method is not MethodDeclarationSyntax methodDeclaration)
            {
                _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Err, $"[{parent.Name}]: ������ ������� �������� �������");
                continue;
            }
            var methodModel = BuildWrapper(method, semanticModel);
            if(methodModel == null)
            {
                _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Warn, $"[{parent.Name}]: ������ ��� ������ �� ������� [{method}]", LogLevelNode.Start);
                continue;
            }

            var context = _methodContextInfoBuilder.BuildContextInfo(parent, method, semanticModel);
            if(context != null)
            {
                result.Add((context, method));
            }
        }

        _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        return result;
    }

    private CSharpMethodSyntaxWrapper? BuildWrapper(MethodDeclarationSyntax syntax, ISemanticModelWrapper model)
    {
        var symbol = CSharpSymbolLoader.LoadSymbol(syntax, model, _onWriteLog, CancellationToken.None);
        if(symbol == null)
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
