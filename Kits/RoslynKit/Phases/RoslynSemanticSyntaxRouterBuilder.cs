using System.Collections.Generic;
using ContextBrowserKit.Options;
using ContextKit.ContextData.Comment;
using ContextKit.Model;
using LoggerKit;
using RoslynKit.Phases.ContextInfoBuilder;
using RoslynKit.Phases.Syntax.Parsers;
using SemanticKit.Model;

namespace RoslynKit.Phases;

public class RoslynSemanticSyntaxRouterBuilder<TContext> : ISemanticSyntaxRouterBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly IContextCollector<TContext> _collector;
    private readonly IContextInfoCommentProcessor<TContext> _commentProcessor;
    private readonly IAppLogger<AppLevel> _logger;
    private readonly ContextInfoBuilderDispatcher<TContext> _contextInfoBuilderDispatcher;

    public RoslynSemanticSyntaxRouterBuilder(
        IContextCollector<TContext> collector,
        IContextInfoCommentProcessor<TContext> commentProcessor,
        ContextInfoBuilderDispatcher<TContext> contextInfoBuilderDispatcher,
        IAppLogger<AppLevel> logger)
    {
        _collector = collector;
        _commentProcessor = commentProcessor;
        _logger = logger;
        _contextInfoBuilderDispatcher = contextInfoBuilderDispatcher;
    }

    public ISemanticSyntaxRouter<TContext> CreateRouter()
    {
        // 1. Создаем билдеры контекстов и другие вспомогательные классы
        var triviaCommentParser = new CSharpSyntaxParserCommentTrivia<TContext>(_commentProcessor, _logger);

        var propertyDeclarationParser = new CSharpSyntaxParserTypeProperty<TContext>(
            _collector,
            triviaCommentParser,
            _contextInfoBuilderDispatcher,
            _logger);

        var enumDeclarationParser = new CSharpSyntaxParserEnum<TContext>(_logger);

        var delegateDeclarationParser = new CSharpSyntaxParserDelegate<TContext>(
            triviaCommentParser,
            _contextInfoBuilderDispatcher,
            _logger);

        var methodSyntaxParser = new CSharpSyntaxParserMethod<TContext>(
            triviaCommentParser,
            _contextInfoBuilderDispatcher,
            _logger);

        var interfaceDeclarationParser = new CSharpSyntaxParserInterface<TContext>(
            propertyDeclarationParser,
            methodSyntaxParser,
            triviaCommentParser,
            _contextInfoBuilderDispatcher,
            _logger);

        var typeDeclarationParser = new CSharpSyntaxParserTypeClass<TContext>(
            propertyDeclarationParser,
            methodSyntaxParser,
            triviaCommentParser,
            _contextInfoBuilderDispatcher,
            _logger);

        var recordDeclarationParser = new CSharpSyntaxParserTypeRecord<TContext>(
            propertyDeclarationParser,
            methodSyntaxParser,
            triviaCommentParser,
            _contextInfoBuilderDispatcher,
            _logger);

        // 4. Собираем все парсеры в список для роутера
        var parsers = new List<ISyntaxParser<TContext>>
        {
            interfaceDeclarationParser,
            typeDeclarationParser,
            enumDeclarationParser,
            propertyDeclarationParser,
            delegateDeclarationParser,
            recordDeclarationParser
        };

        return new SemanticSyntaxRouter<TContext>(_logger, parsers);
    }
}
