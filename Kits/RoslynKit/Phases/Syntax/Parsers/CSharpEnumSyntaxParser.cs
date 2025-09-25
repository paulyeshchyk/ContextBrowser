using System.Threading;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Phases.Syntax.Parsers;

public class CSharpEnumSyntaxParser<TContext> : BaseSyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpEnumSyntaxParser(IAppLogger<AppLevel> logger)
        : base(logger)
    {
    }

    public override bool CanParse(object syntax) => syntax is EnumDeclarationSyntax;

    public override void Parse(TContext? parent, object syntax, ISemanticModelWrapper model, SemanticOptions options, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Trace, $"Syntax type is not parsed yet: {syntax.GetType()}");
    }
}