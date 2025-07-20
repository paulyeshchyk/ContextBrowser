using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ContextBrowser.Parser.Roslyn;

internal static class RoslynParser
{
    // context: roslyn, csharp, build, file
    public static List<ContextInfo> ParseFile(string filePath, ParserOptions? options = null)
    {
        options ??= ParserOptions.Default;

        var code = File.ReadAllText(filePath);
        var tree = CSharpSyntaxTree.ParseText(code);
        var root = tree.GetCompilationUnitRoot();

        var result = new List<ContextInfo>();

        var classNodes = root
            .DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .Where(cls =>
            {
                var modifier = GetClassModifierType(cls);
                return modifier.HasValue && options.ClassModifierTypes.Contains(modifier.Value);
            });

        foreach(var cls in classNodes)
        {
            // Получаем namespace, если есть
            var nsNode = cls.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault();
            var nsName = nsNode?.Name.ToString() ?? "Global";

            ProcessClassNode(cls, nsName, result, options);
        }

        return result;
    }

    private static void ProcessClassNode(ClassDeclarationSyntax cls, string nsName, List<ContextInfo> result, ParserOptions options)
    {
        var className = cls.Identifier.Text;

        var classInfo = BuildContextInfo(cls, "class", nsName, className);
        result.Add(classInfo);

        var methodNodes = cls
            .DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .Where(method =>
            {
                var modifier = GetModifierType(method);
                return modifier.HasValue && options.MethodModifierTypes.Contains(modifier.Value);
            });

        foreach(var method in methodNodes)
        {
            var methodName = method.Identifier.Text;
            var fullName = $"{className}.{methodName}";
            var methodInfo = BuildContextInfo(method, "method", nsName, className, fullName);
            result.Add(methodInfo);
        }
    }

    private static AccessorModifierType? GetModifierType(MethodDeclarationSyntax method)
    {
        if(method.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
            return AccessorModifierType.@public;
        if(method.Modifiers.Any(m => m.IsKind(SyntaxKind.ProtectedKeyword)))
            return AccessorModifierType.@protected;
        if(method.Modifiers.Any(m => m.IsKind(SyntaxKind.PrivateKeyword)))
            return AccessorModifierType.@private;
        if(method.Modifiers.Any(m => m.IsKind(SyntaxKind.InternalKeyword)))
            return AccessorModifierType.@internal;

        // Если нет модификаторов, можно трактовать как internal по умолчанию (или private — зависит от контекста)
        return null;
    }

    private static AccessorModifierType? GetClassModifierType(ClassDeclarationSyntax cls)
    {
        if(cls.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
            return AccessorModifierType.@public;
        if(cls.Modifiers.Any(m => m.IsKind(SyntaxKind.ProtectedKeyword)))
            return AccessorModifierType.@protected;
        if(cls.Modifiers.Any(m => m.IsKind(SyntaxKind.PrivateKeyword)))
            return AccessorModifierType.@private;
        if(cls.Modifiers.Any(m => m.IsKind(SyntaxKind.InternalKeyword)))
            return AccessorModifierType.@internal;

        // По умолчанию, если нет модификаторов — internal для namespace-level классов
        return null;
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

public record ParserOptions(HashSet<AccessorModifierType> MethodModifierTypes, HashSet<AccessorModifierType> ClassModifierTypes)
{
    public static ParserOptions Default => new(
        new HashSet<AccessorModifierType>
        {
            AccessorModifierType.@public,
            AccessorModifierType.@protected
        },
        new HashSet<AccessorModifierType>
        {
            AccessorModifierType.@public,
            AccessorModifierType.@protected
        });
}

public enum AccessorModifierType
{
    @public,
    @protected,
    @private,
    @internal
}