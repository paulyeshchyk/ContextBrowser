using System.Collections.Generic;
using ContextBrowserKit.Options;
using ContextKit.ContextData.Comment;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit;
using RoslynKit.Assembly;
using RoslynKit.Phases.Syntax.Parsers;
using SemanticKit.Model;

namespace RoslynKit.Assembly;

// context: roslyn, build
public class RoslynSemanticSyntaxRouterBuilder<TContext> : ISemanticSyntaxRouterBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly IContextCollector<TContext> _collector;
    private readonly IContextInfoCommentProcessor<TContext> _commentProcessor;
    private readonly IAppLogger<AppLevel> _logger;
    private readonly ContextInfoBuilderDispatcher<TContext> _contextInfoBuilderDispatcher;
    private readonly ISymbolLoader<MemberDeclarationSyntax, ISymbol> _symbolLoader;

    public RoslynSemanticSyntaxRouterBuilder(
        IContextCollector<TContext> collector,
        IContextInfoCommentProcessor<TContext> commentProcessor,
        ContextInfoBuilderDispatcher<TContext> contextInfoBuilderDispatcher,
        IAppLogger<AppLevel> logger,
        ISymbolLoader<MemberDeclarationSyntax, ISymbol> symbolLoader)
    {
        _collector = collector;
        _commentProcessor = commentProcessor;
        _logger = logger;
        _contextInfoBuilderDispatcher = contextInfoBuilderDispatcher;
        _symbolLoader = symbolLoader;

    }

    // context: roslyn, build, syntax
    public ISemanticSyntaxRouter<TContext> CreateRouter()
    {
        // 1. Создаем билдеры контекстов и другие вспомогательные классы
        var triviaCommentParser = new CSharpSyntaxParserCommentTrivia<TContext>(_commentProcessor, _logger);

        var propertySyntaxParser = new CSharpSyntaxParserTypeProperty<TContext>(
            _collector,
            triviaCommentParser,
            _contextInfoBuilderDispatcher,
            _logger);

        var enumSyntaxParser = new CSharpSyntaxParserEnum<TContext>(_logger);

        var delegateSyntaxParser = new CSharpSyntaxParserDelegate<TContext>(
            triviaCommentParser,
            _contextInfoBuilderDispatcher,
            _logger);

        var methodSyntaxParser = new CSharpSyntaxParserMethod<TContext>(
            triviaCommentParser,
            _contextInfoBuilderDispatcher,
            _logger,
            _symbolLoader);

        var interfaceSyntaxParser = new CSharpSyntaxParserInterface<TContext>(
            propertySyntaxParser,
            methodSyntaxParser,
            triviaCommentParser,
            _contextInfoBuilderDispatcher,
            _logger);

        var typeDeclarationParser = new CSharpSyntaxParserTypeClass<TContext>(
            propertySyntaxParser,
            methodSyntaxParser,
            triviaCommentParser,
            _contextInfoBuilderDispatcher,
            _logger,
            _symbolLoader);

        var recordSyntaxParser = new CSharpSyntaxParserTypeRecord<TContext>(
            propertySyntaxParser,
            methodSyntaxParser,
            triviaCommentParser,
            _contextInfoBuilderDispatcher,
            _logger);

        // 4. Собираем все парсеры в список для роутера
        var parsers = new List<ISyntaxParser<TContext>>
        {
            interfaceSyntaxParser,
            typeDeclarationParser,
            enumSyntaxParser,
            propertySyntaxParser,
            delegateSyntaxParser,
            recordSyntaxParser
        };

        return new SemanticSyntaxRouter<TContext>(_logger, parsers);
    }
}
