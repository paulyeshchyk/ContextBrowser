using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynKit.Parser.Syntaxies.Parser;

// Парсер-заглушка для DelegateDeclarationSyntax
public class DelegateDeclarationParser<TContext> : BaseSyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    public DelegateDeclarationParser(OnWriteLog? onWriteLog)
        : base(onWriteLog) { }

    public override bool CanParse(MemberDeclarationSyntax syntax) => syntax is DelegateDeclarationSyntax;

    public override void Parse(MemberDeclarationSyntax syntax, SemanticModel model)
    {
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"Syntax type is not parsed yet: {syntax.GetType()}");
    }
}