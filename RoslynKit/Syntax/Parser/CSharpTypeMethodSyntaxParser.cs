using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;
using RoslynKit.Model;
using RoslynKit.Semantic.Builder;

namespace RoslynKit.Syntax.Parser;

public class CSharpTypeMethodSyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private RoslynCodeParserOptions _options;
    private OnWriteLog? _onWriteLog;
    private MethodContextInfoBuilder<TContext> _methodContextInfoBuilder;
    private CSharpCommentTriviaSyntaxParser<TContext> _triviaCommentParser;

    public CSharpTypeMethodSyntaxParser(MethodContextInfoBuilder<TContext> methodContextInfoBuilder, CSharpCommentTriviaSyntaxParser<TContext> triviaCommentParser, RoslynCodeParserOptions options, OnWriteLog? onWriteLog)
    {
        _options = options;
        _onWriteLog = onWriteLog;
        _methodContextInfoBuilder = methodContextInfoBuilder;
        _triviaCommentParser = triviaCommentParser;
    }

    //context: csharp, build
    public void ParseMethodSyntax(MemberDeclarationSyntax availableSyntax, SemanticModel semanticModel, string nsName, TContext typeContext)
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

        var buildItems = _methodContextInfoBuilder.BuildContextInfoForMethods(semanticModel, nsName, typeContext, methodDeclarationSyntaxies);

        foreach(var item in buildItems)
        {
            _triviaCommentParser.Parse(item.Item2, item.Item1);
        }
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
