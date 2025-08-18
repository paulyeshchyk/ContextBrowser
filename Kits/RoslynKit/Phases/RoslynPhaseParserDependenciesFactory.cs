using ContextBrowserKit.Log;
using ContextKit.Model;
using RoslynKit.Basics.Comment;
using RoslynKit.Model;
using RoslynKit.Semantic.Builder;
using RoslynKit.Syntax;
using RoslynKit.Syntax.Parser;

namespace RoslynKit.Phases;

public class RoslynPhaseParserDependenciesFactory<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly IContextCollector<TContext> _collector;
    private readonly IContextFactory<TContext> _factory;
    private readonly IContextInfoCommentProcessor<TContext> _commentProcessor;
    private readonly RoslynCodeParserOptions _options;
    private readonly OnWriteLog? _onWriteLog;

    public RoslynPhaseParserDependenciesFactory(
        IContextCollector<TContext> collector,
        IContextFactory<TContext> factory,
        IContextInfoCommentProcessor<TContext> commentProcessor,
        RoslynCodeParserOptions options,
        OnWriteLog? onWriteLog)
    {
        _collector = collector;
        _factory = factory;
        _commentProcessor = commentProcessor;
        _options = options;
        _onWriteLog = onWriteLog;
    }

    public SyntaxRouter<TContext> CreateRouter()
    {
        // 1. Создаем билдеры контекстов и другие вспомогательные классы
        var typeContextInfoBuilder = new CSharpTypeContextInfoBulder<TContext>(_collector, _factory, _onWriteLog);
        var methodContextInfoBuilder = new CSharpMethodContextInfoBuilder<TContext>(_collector, _factory, _onWriteLog);
        var triviaCommentParser = new CSharpCommentTriviaSyntaxParser<TContext>(_commentProcessor, _onWriteLog);
        var delegateContextInfoBuilder = new CSharpDelegateContextInfoBuilder<TContext>(_collector, _factory, _onWriteLog);
        var interfaceContextInfoBuilder = new CSharpInterfaceContextInfoBuilder<TContext>(_collector, _factory, _onWriteLog);
        var propertyContextInfoBuilder = new CSharpPropertyContextInfoBuilder<TContext>(_collector, _factory, _onWriteLog);
        var recordContextInfoBuilder = new CSharpRecordContextInfoBuilder<TContext>(_collector, _factory, _onWriteLog);
        var enumContextInfoBuilder = new CSharpEnumContextInfoBuilder<TContext>(_collector, _factory, _onWriteLog);

        var propertyDeclarationParser = new CSharpTypePropertyParser<TContext>(
            _collector,
            propertyContextInfoBuilder,
            triviaCommentParser,
            typeContextInfoBuilder,
            recordContextInfoBuilder,
            enumContextInfoBuilder,
            interfaceContextInfoBuilder,
            _onWriteLog);

        var enumDeclarationParser = new CSharpEnumSyntaxParser<TContext>(_onWriteLog);
        var delegateDeclarationParser = new CSharpDelegateSyntaxParser<TContext>(delegateContextInfoBuilder, triviaCommentParser, _onWriteLog);

        var methodSyntaxParser = new CSharpMethodSyntaxParser<TContext>(methodContextInfoBuilder, triviaCommentParser, _options, _onWriteLog);

        var interfaceDeclarationParser = new CSharpInterfaceSyntaxParser<TContext>(
            interfaceContextInfoBuilder,
            propertyDeclarationParser,
            methodSyntaxParser,
            triviaCommentParser,
            _onWriteLog);

        var typeDeclarationParser = new CSharpTypeClassSyntaxParser<TContext>(
            _collector,
            typeContextInfoBuilder,
            propertyDeclarationParser,
            methodSyntaxParser,
            triviaCommentParser,
            _options,
            _onWriteLog);

        var recordDeclarationParser = new CSharpTypeRecordSyntaxParser<TContext>(
            _collector,
            recordContextInfoBuilder,
            propertyDeclarationParser,
            methodSyntaxParser,
            triviaCommentParser,
            _options,
            _onWriteLog);


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

        return new SyntaxRouter<TContext>(_onWriteLog, parsers);
    }
}
