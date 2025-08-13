using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynKit.Syntax.Parser;

public class CSharpEnumSyntaxParser<TContext> : BaseSyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpEnumSyntaxParser(OnWriteLog? onWriteLog)
        : base(onWriteLog)
    {
    }

    public override bool CanParse(MemberDeclarationSyntax syntax) => syntax is EnumDeclarationSyntax;

    public override void Parse(TContext? parent, MemberDeclarationSyntax syntax, SemanticModel model)
    {
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Trace, $"Syntax type is not parsed yet: {syntax.GetType()}");
    }
}