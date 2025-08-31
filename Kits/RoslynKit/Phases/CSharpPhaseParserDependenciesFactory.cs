using ContextBrowserKit.Log;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Stategies;
using LoggerKit;
using RoslynKit.Phases.ContextInfoBuilder;
using RoslynKit.Phases.Syntax;
using RoslynKit.Phases.Syntax.Parsers;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Phases;

public interface ISemanticSyntaxRouterBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
    ISemanticSyntaxRouter<TContext> CreateRouter(SemanticOptions options);
}

public class CSharpPhaseParserDependenciesFactory<TContext> : ISemanticSyntaxRouterBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly IContextCollector<TContext> _collector;
    private readonly IContextFactory<TContext> _factory;
    private readonly IContextInfoCommentProcessor<TContext> _commentProcessor;
    private readonly IAppLogger<AppLevel> _logger;

    public CSharpPhaseParserDependenciesFactory(
        IContextCollector<TContext> collector,
        IContextFactory<TContext> factory,
        IContextInfoCommentProcessor<TContext> commentProcessor,
        IAppLogger<AppLevel> logger)
    {
        _collector = collector;
        _factory = factory;
        _commentProcessor = commentProcessor;
        _logger = logger;
    }

    public ISemanticSyntaxRouter<TContext> CreateRouter(SemanticOptions options)
    {
        // 1. Создаем билдеры контекстов и другие вспомогательные классы
        var typeContextInfoBuilder = new CSharpTypeContextInfoBulder<TContext>(_collector, _factory, _logger.WriteLog);
        var methodContextInfoBuilder = new CSharpMethodContextInfoBuilder<TContext>(_collector, _factory, _logger.WriteLog);
        var triviaCommentParser = new CSharpCommentTriviaSyntaxParser<TContext>(_commentProcessor, _logger.WriteLog);
        var delegateContextInfoBuilder = new CSharpDelegateContextInfoBuilder<TContext>(_collector, _factory, _logger.WriteLog);
        var interfaceContextInfoBuilder = new CSharpInterfaceContextInfoBuilder<TContext>(_collector, _factory, _logger.WriteLog);
        var propertyContextInfoBuilder = new CSharpPropertyContextInfoBuilder<TContext>(_collector, _factory, _logger.WriteLog);
        var recordContextInfoBuilder = new CSharpRecordContextInfoBuilder<TContext>(_collector, _factory, _logger.WriteLog);
        var enumContextInfoBuilder = new CSharpEnumContextInfoBuilder<TContext>(_collector, _factory, _logger.WriteLog);

        var propertyDeclarationParser = new CSharpTypePropertyParser<TContext>(
            _collector,
            propertyContextInfoBuilder,
            triviaCommentParser,
            typeContextInfoBuilder,
            recordContextInfoBuilder,
            enumContextInfoBuilder,
            interfaceContextInfoBuilder,
            _logger.WriteLog);

        var enumDeclarationParser = new CSharpEnumSyntaxParser<TContext>(_logger.WriteLog);
        var delegateDeclarationParser = new CSharpDelegateSyntaxParser<TContext>(delegateContextInfoBuilder, triviaCommentParser, _logger.WriteLog);

        var methodSyntaxParser = new CSharpMethodSyntaxParser<TContext>(methodContextInfoBuilder, triviaCommentParser, _logger.WriteLog);

        var interfaceDeclarationParser = new CSharpInterfaceSyntaxParser<TContext>(
            interfaceContextInfoBuilder,
            propertyDeclarationParser,
            methodSyntaxParser,
            triviaCommentParser,
            _logger.WriteLog);

        var typeDeclarationParser = new CSharpTypeClassSyntaxParser<TContext>(
            _collector,
            typeContextInfoBuilder,
            propertyDeclarationParser,
            methodSyntaxParser,
            triviaCommentParser,
            options,
            _logger.WriteLog);

        var recordDeclarationParser = new CSharpTypeRecordSyntaxParser<TContext>(
            _collector,
            recordContextInfoBuilder,
            propertyDeclarationParser,
            methodSyntaxParser,
            triviaCommentParser,
            options,
            _logger.WriteLog);

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

        return new SemanticSyntaxRouter<TContext>(_logger.WriteLog, parsers);
    }
}
