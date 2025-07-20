using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ContextBrowser.Parser.Roslyn;

internal static class RoslynParser
{
    // context: roslyn, csharp, build, file
    public static List<ContextInfo> ParseFile(string filePath, RoslynParserOptions? options = null)
    {
        options ??= RoslynParserOptions.Default;

        var code = File.ReadAllText(filePath);
        var tree = CSharpSyntaxTree.ParseText(code);
        var root = tree.GetCompilationUnitRoot();

        var result = new List<ContextInfo>();

        IEnumerable<MemberDeclarationSyntax> typeNodes = Enumerable.Empty<MemberDeclarationSyntax>();
        if(options.MemberTypes.Contains(RoslynMemberType.@class))
            typeNodes = typeNodes.Concat(FilterByModifier<ClassDeclarationSyntax>(root, options));
        if(options.MemberTypes.Contains(RoslynMemberType.@record))
            typeNodes = typeNodes.Concat(FilterByModifier<RecordDeclarationSyntax>(root, options));
        if(options.MemberTypes.Contains(RoslynMemberType.@struct))
            typeNodes = typeNodes.Concat(FilterByModifier<StructDeclarationSyntax>(root, options));
        if(options.MemberTypes.Contains(RoslynMemberType.@enum))
            typeNodes = typeNodes.Concat(FilterByModifier<EnumDeclarationSyntax>(root, options));

        foreach(var cls in typeNodes)
        {
            // Получаем namespace, если есть
            var nsNode = cls.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault();
            var nsName = nsNode?.Name.ToString() ?? "Global";

            ProcessClassNode(cls, nsName, result, options);
        }

        return result;
    }

    private static string GetDeclarationName(MemberDeclarationSyntax cls)
    {
        if(cls is ClassDeclarationSyntax syntaxClass)
            return syntaxClass.Identifier.Text;
        if(cls is TypeDeclarationSyntax syntaxStruct)
            return syntaxStruct.Identifier.Text;
        if(cls is EnumDeclarationSyntax syntaxEnum)
            return syntaxEnum.Identifier.Text;
        if(cls is RecordDeclarationSyntax syntaxRecord)
            return syntaxRecord.Identifier.Text;
        return "?????";
    }

    private static void ProcessClassNode(MemberDeclarationSyntax cls, string nsName, List<ContextInfo> result, RoslynParserOptions options)
    {
        var className = GetDeclarationName(cls);

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

    private static IEnumerable<T> FilterByModifier<T>(SyntaxNode root, RoslynParserOptions options)
    where T : MemberDeclarationSyntax
    {
        return root
            .DescendantNodes()
            .OfType<T>()
            .Where(node =>
            {
                var modifier = GetClassModifierType(node);
                return modifier.HasValue && options.ClassModifierTypes.Contains(modifier.Value);
            });
    }

    private static RoslynAccessorModifierType? GetModifierType(MethodDeclarationSyntax method)
    {
        if(method.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
            return RoslynAccessorModifierType.@public;
        if(method.Modifiers.Any(m => m.IsKind(SyntaxKind.ProtectedKeyword)))
            return RoslynAccessorModifierType.@protected;
        if(method.Modifiers.Any(m => m.IsKind(SyntaxKind.PrivateKeyword)))
            return RoslynAccessorModifierType.@private;
        if(method.Modifiers.Any(m => m.IsKind(SyntaxKind.InternalKeyword)))
            return RoslynAccessorModifierType.@internal;

        // Если нет модификаторов, можно трактовать как internal по умолчанию (или private — зависит от контекста)
        return null;
    }

    private static RoslynAccessorModifierType? GetClassModifierType<T>(T member)
        where T : MemberDeclarationSyntax
    {
        if(member.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
            return RoslynAccessorModifierType.@public;
        if(member.Modifiers.Any(m => m.IsKind(SyntaxKind.ProtectedKeyword)))
            return RoslynAccessorModifierType.@protected;
        if(member.Modifiers.Any(m => m.IsKind(SyntaxKind.PrivateKeyword)))
            return RoslynAccessorModifierType.@private;
        if(member.Modifiers.Any(m => m.IsKind(SyntaxKind.InternalKeyword)))
            return RoslynAccessorModifierType.@internal;

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

            ParseComment(info, comment);
        }

        return info;
    }

    private static void ParseComment(ContextInfo info, string comment)
    {
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
}

internal record RoslynParserOptions(HashSet<RoslynAccessorModifierType> MethodModifierTypes, HashSet<RoslynAccessorModifierType> ClassModifierTypes, HashSet<RoslynMemberType> MemberTypes)
{
    public static RoslynParserOptions Default => new(
        new HashSet<RoslynAccessorModifierType>
        {
            RoslynAccessorModifierType.@public,
            RoslynAccessorModifierType.@protected
        },
        new HashSet<RoslynAccessorModifierType>
        {
            RoslynAccessorModifierType.@public,
            RoslynAccessorModifierType.@protected
        },
        new HashSet<RoslynMemberType>
        {
            RoslynMemberType.@enum,
            RoslynMemberType.@class,
            RoslynMemberType.@record,
            RoslynMemberType.@struct
        }
    );
}

public enum RoslynMemberType
{
    @class,
    @record,
    @enum,
    @struct
}

public enum RoslynAccessorModifierType
{
    @public,
    @protected,
    @private,
    @internal
}