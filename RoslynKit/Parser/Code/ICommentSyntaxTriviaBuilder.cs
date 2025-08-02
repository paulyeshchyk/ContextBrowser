using ContextKit.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynKit.Parser.Code;

public interface ICommentSyntaxBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
    void ParseComments(MemberDeclarationSyntax node, TContext context);
}
