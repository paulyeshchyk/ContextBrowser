using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;
using RoslynKit.Model;
using RoslynKit.Semantic.Builder;
using RoslynKit.Syntax.AssemblyLoader;
using RoslynKit.Syntax.Parser.Wrappers;

namespace RoslynKit.Syntax.Parser;

public class CSharpMethodSyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private RoslynCodeParserOptions _options;
    private OnWriteLog? _onWriteLog;
    private CSharpMethodContextInfoBuilder<TContext> _methodContextInfoBuilder;
    private CSharpCommentTriviaSyntaxParser<TContext> _triviaCommentParser;

    public CSharpMethodSyntaxParser(CSharpMethodContextInfoBuilder<TContext> methodContextInfoBuilder, CSharpCommentTriviaSyntaxParser<TContext> triviaCommentParser, RoslynCodeParserOptions options, OnWriteLog? onWriteLog)
    {
        _options = options;
        _onWriteLog = onWriteLog;
        _methodContextInfoBuilder = methodContextInfoBuilder;
        _triviaCommentParser = triviaCommentParser;
    }

    //context: csharp, build
    public void ParseMethodSyntax(MemberDeclarationSyntax availableSyntax, SemanticModel semanticModel, TContext typeContext)
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


    public List<(TContext context, MethodDeclarationSyntax syntax)> ParseMethodSyntax(TContext parent, IEnumerable<MethodDeclarationSyntax> methods, SemanticModel semanticModel)
    {
        var result = new List<(TContext, MethodDeclarationSyntax)>();
        _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Dbg, $"Iterating methods [{parent.Name}]", LogLevelNode.Start);

        foreach(var method in methods)
        {
            if(method is not MethodDeclarationSyntax methodDeclaration)
            {
                _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Err, $"[{parent.Name}]: Списко методов содержит ПУСТОТУ");
                continue;
            }
            var methodModel = BuildWrapper(method, semanticModel);
            if(methodModel == null)
            {
                _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Warn, $"[{parent.Name}]: Модель для метода не найдена [{method}]", LogLevelNode.Start);
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

    private CSharpMethodSyntaxWrapper? BuildWrapper(MethodDeclarationSyntax syntax, SemanticModel model)
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
    public static IEnumerable<MethodDeclarationSyntax> FilteredMethodsList(this TypeDeclarationSyntax typeSyntax, RoslynCodeParserOptions options)
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
