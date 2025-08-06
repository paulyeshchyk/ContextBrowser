using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynKit.Syntax.Parser;

public class EnumDeclarationParser<TContext> : BaseSyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    public EnumDeclarationParser(OnWriteLog? onWriteLog)
        : base(onWriteLog)
    {
    }

    public override bool CanParse(MemberDeclarationSyntax syntax) => syntax is EnumDeclarationSyntax;

    public override void Parse(MemberDeclarationSyntax syntax, SemanticModel model)
    {
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"Syntax type is not parsed yet: {syntax.GetType()}");
    }
}