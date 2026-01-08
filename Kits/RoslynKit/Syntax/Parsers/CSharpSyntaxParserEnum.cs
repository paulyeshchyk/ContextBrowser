using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Phases.Syntax.Parsers;

// context: syntax, build, roslyn
public class CSharpSyntaxParserEnum<TContext> : SyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpSyntaxParserEnum(IAppLogger<AppLevel> logger)
        : base(logger)
    {
    }

    public override bool CanParseSyntax(object syntax) => syntax is EnumDeclarationSyntax;

    public override Task ParseAsync(TContext? parent, object syntax, ISemanticModelWrapper model, SemanticOptions options, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Trace, $"Syntax type is not parsed yet: {syntax.GetType()}");
        return Task.CompletedTask;
    }
}