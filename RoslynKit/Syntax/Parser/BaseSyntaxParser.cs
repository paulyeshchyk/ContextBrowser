using ContextBrowserKit.Log;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynKit.Syntax.Parser;

public abstract class BaseSyntaxParser<TContext> : ISyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    protected readonly OnWriteLog? _onWriteLog;

    protected BaseSyntaxParser(OnWriteLog? onWriteLog)
    {
        _onWriteLog = onWriteLog;
    }

    public abstract bool CanParse(MemberDeclarationSyntax syntax);

    public abstract void Parse(MemberDeclarationSyntax syntax, SemanticModel model);
}