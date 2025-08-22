using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SemanticKit.Model;

namespace RoslynKit.Phases.Syntax.Parsers;

public class CSharpEnumSyntaxParser<TContext> : BaseSyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpEnumSyntaxParser(OnWriteLog? onWriteLog)
        : base(onWriteLog)
    {
    }

    public override bool CanParse(object syntax) => syntax is EnumDeclarationSyntax;

    public override void Parse(TContext? parent, object syntax, ISemanticModelWrapper model, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Trace, $"Syntax type is not parsed yet: {syntax.GetType()}");
    }
}