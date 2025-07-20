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

        var classNodes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

        foreach(var cls in classNodes)
        {
            // Получаем namespace, если есть
            var nsNode = cls.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault();
            var nsName = nsNode?.Name.ToString() ?? "Global";

            ProcessClassNode(cls, nsName, result);
        }

        return result;
    }

    private static void ProcessClassNode(ClassDeclarationSyntax cls, string nsName, List<ContextInfo> result)
    {
        var className = cls.Identifier.Text;

        var classInfo = BuildContextInfo(cls, "class", nsName, className);
        result.Add(classInfo);

        var methodNodes = cls.DescendantNodes().OfType<MethodDeclarationSyntax>();

        foreach(var method in methodNodes)
        {
            var methodName = method.Identifier.Text;
            var fullName = $"{className}.{methodName}";
            var methodInfo = BuildContextInfo(method, "method", nsName, className, fullName);
            result.Add(methodInfo);
        }
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