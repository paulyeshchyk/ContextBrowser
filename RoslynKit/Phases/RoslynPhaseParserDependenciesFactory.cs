using ContextBrowser.ContextKit.Parser;
using ContextKit.Model;
using LoggerKit;
using RoslynKit.Model;
using RoslynKit.Syntax;
using RoslynKit.Syntax.Parser.Base;
using RoslynKit.Syntax.Parser.ContextInfo;

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
        var typeContextInfoBuilder = new TypeContextInfoBulder<TContext>(_collector, _factory, _onWriteLog);
        var methodContextInfoBuilder = new MethodContextInfoBuilder<TContext>(_collector, _factory, _onWriteLog);
        var triviaCommentBuilder = new CommentSyntaxTriviaContentInfoBuilder<TContext>(_commentProcessor, _onWriteLog);
        var delegateContextInfoBuilder = new DelegateContextInfoBuilder<TContext>(_collector, _factory, _onWriteLog);
        var interfaceContextInfoBuilder = new InterfaceContextInfoBuilder<TContext>(_collector, _factory, _onWriteLog);
        var propertyContextInfoBuilder = new PropertyContextInfoBuilder<TContext>(_collector, _factory, _onWriteLog);
        var recordContextInfoBuilder = new RecordContextInfoBuilder<TContext>(_collector, _factory, _onWriteLog);
        var enumContextInfoBuilder = new EnumContextInfoBuilder<TContext>(_collector, _factory, _onWriteLog);

        var propertyDeclarationParser = new PropertyDeclarationParser<TContext>(
            _collector,
            propertyContextInfoBuilder,
            triviaCommentBuilder,
            typeContextInfoBuilder,
            recordContextInfoBuilder,
            enumContextInfoBuilder,
            _onWriteLog);

        var enumDeclarationParser = new EnumDeclarationParser<TContext>(_onWriteLog);
        var delegateDeclarationParser = new DelegateDeclarationParser<TContext>(delegateContextInfoBuilder, triviaCommentBuilder, _onWriteLog);

        var interfaceDeclarationParser = new InterfaceDeclarationParser<TContext>(
            interfaceContextInfoBuilder,
            methodContextInfoBuilder,
            triviaCommentBuilder,
            _onWriteLog);

        var typeDeclarationParser = new TypeDeclarationParser<TContext>(
            _collector,
            typeContextInfoBuilder,
            methodContextInfoBuilder,
            propertyDeclarationParser,
            triviaCommentBuilder,
            _options,
            _onWriteLog);

        var recordDeclarationParser = new RecordDeclarationParser<TContext>(
            _collector,
            recordContextInfoBuilder,
            propertyDeclarationParser,
            triviaCommentBuilder,
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
