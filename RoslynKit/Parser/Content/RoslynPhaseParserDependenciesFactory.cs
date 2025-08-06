using ContextBrowser.ContextKit.Parser;
using ContextKit.Model;
using LoggerKit;
using RoslynKit.Model;
using RoslynKit.Model.Builder;
using RoslynKit.Parser.Code;
using RoslynKit.Parser.Syntaxies.Parser;

namespace RoslynKit.Parser.Content;

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
        var typeContextInfoBuilder = new TypeContextInfoBulder<TContext>(_collector, _factory, _onWriteLog);
        var methodContextInfoBuilder = new MethodContextInfoBuilder<TContext>(_collector, _factory, _onWriteLog);
        var triviaContextInfoBuilder = new CommentSyntaxTriviaBuilder<TContext>(_commentProcessor);

        var parsers = new List<ISyntaxParser<TContext>>
        {
            new TypeSyntaxParser<TContext>(_collector, typeContextInfoBuilder, methodContextInfoBuilder,triviaContextInfoBuilder, _options,_onWriteLog),
            new EnumDeclarationParser<TContext>(_onWriteLog),
            new DelegateDeclarationParser<TContext>(_onWriteLog)
        };

        return new SyntaxRouter<TContext>(_onWriteLog, parsers);
    }
}
