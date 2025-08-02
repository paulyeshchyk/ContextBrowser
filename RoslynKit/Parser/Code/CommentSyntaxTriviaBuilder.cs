using ContextBrowser.ContextKit.Parser;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynKit.Parser.Code;

public class CommentSyntaxTriviaBuilder<TContext> : ICommentSyntaxBuilder<TContext>
        where TContext : IContextWithReferences<TContext>
{
    IContextInfoCommentProcessor<TContext> _commentAdapter;

    public CommentSyntaxTriviaBuilder(IContextInfoCommentProcessor<TContext> commentProcessor)
    {
        _commentAdapter = commentProcessor;
    }

    // context: csharp, build
    public void ParseComments(MemberDeclarationSyntax node, TContext context)
    {
        foreach (var trivia in node.GetLeadingTrivia().Where(t => t.IsKind(SyntaxKind.SingleLineCommentTrivia)))
        {
            string comment = ExtractComment(trivia);
            _commentAdapter.Process(comment, context);
        }
    }

    // context: csharp, build
    internal string ExtractComment(SyntaxTrivia trivia)
    {
        return trivia.ToString()
                     .TrimStart('/')
                     .Trim();
    }
}