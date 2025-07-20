using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ContextBrowser.Parser.Roslyn;

internal static class RoslynParser
{
    public static List<ContextInfo> ParseFile(string filePath)
    {
        var code = File.ReadAllText(filePath);
        var tree = CSharpSyntaxTree.ParseText(code);
        var root = tree.GetCompilationUnitRoot();

        var result = new List<ContextInfo>();

        foreach(var ns in root.DescendantNodes().OfType<NamespaceDeclarationSyntax>())
        {
            var nsName = ns.Name.ToString();

            foreach(var cls in ns.DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                var className = cls.Identifier.Text;
                var classInfo = BuildContextInfo(cls, "class", nsName, className);
                result.Add(classInfo);

                foreach(var method in cls.DescendantNodes().OfType<MethodDeclarationSyntax>())
                {
                    var methodName = method.Identifier.Text;
                    var fullName = $"{className}.{methodName}";
                    var methodInfo = BuildContextInfo(method, "method", nsName, className, fullName);
                    result.Add(methodInfo);
                }
            }
        }

        return result;
    }

    private static ContextInfo BuildContextInfo(MemberDeclarationSyntax node, string type, string? ns, string? owner, string? fullName = null)
    {
        var info = new ContextInfo
        {
            ElementType = type,
            Name = fullName ?? owner,
            Namespace = ns,
            ClassOwner = type == "method" ? owner : null
        };

        var trivia = node.GetLeadingTrivia();

        foreach(var t in trivia.Where(t => t.IsKind(SyntaxKind.SingleLineCommentTrivia)))
        {
            var comment = t.ToString().TrimStart('/').Trim();

            if(comment.StartsWith("context:", StringComparison.OrdinalIgnoreCase))
            {
                var tags = comment.Substring("context:".Length)
                                  .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                                  .Select(t => t.Trim().ToLowerInvariant());

                foreach(var tag in tags)
                    info.Contexts.Add(tag);
            }

            if(comment.StartsWith("coverage:", StringComparison.OrdinalIgnoreCase))
            {
                var val = comment.Substring("coverage:".Length).Trim();
                info.Dimensions["coverage"] = val;
            }
        }

        return info;
    }
}