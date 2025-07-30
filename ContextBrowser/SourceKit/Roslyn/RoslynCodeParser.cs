using ContextBrowser.ContextKit.Parser;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ContextBrowser.SourceKit.Roslyn;

// context: csharp, read
//public abstract class RoslynCodeParser<TContext>
//    where TContext : IContextWithReferences<TContext>
//{
//    public RoslynCodeParser()
//    {
//    }

//    // context: csharp, read
//    public abstract void ParseCode(string code, string filePath, RoslynCodeParserOptions options, RoslynContextParserOptions contextOptions);

//    // context: csharp, read
//    public abstract void ParseFile(string filePath, RoslynCodeParserOptions options, RoslynContextParserOptions contextOptions);
//}


internal static class SyntaxTriviaExt
{
    public static void ParseSingleLineComments<TContext>(this MemberDeclarationSyntax node, IContextInfoCommentProcessor<TContext> commentProcessor, TContext context)
    {
        foreach(var trivia in node.GetLeadingTrivia().Where(t => t.IsKind(SyntaxKind.SingleLineCommentTrivia)))
        {
            string comment = trivia.ExtractComment();
            commentProcessor.Process(comment, context);
        }
    }

    internal static string ExtractComment(this SyntaxTrivia trivia)
    {
        return trivia.ToString()
                     .TrimStart('/')
                     .Trim();
    }
}
