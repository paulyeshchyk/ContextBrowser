using ContextBrowser.model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ContextBrowser.Parser.Roslyn;

internal class RoslynParser<TContext>
{
    private readonly IContextCollector<TContext> _collector;
    private readonly IContextFactory<TContext> _factory;
    private readonly IContextInfoCommentProcessor<TContext> _commentProcessor;

    public RoslynParser(IContextCollector<TContext> collector, IContextFactory<TContext> factory, IContextInfoCommentProcessor<TContext> commentProcessor)
    {
        _collector = collector;
        _factory = factory;
        _commentProcessor = commentProcessor;
    }

    public void ParseFile(string filePath, RoslynParserOptions? options = null)
    {
        options ??= RoslynParserOptions.Default;

        var code = File.ReadAllText(filePath);
        var tree = CSharpSyntaxTree.ParseText(code);
        var root = tree.GetCompilationUnitRoot();

        IEnumerable<MemberDeclarationSyntax> typeNodes = Enumerable.Empty<MemberDeclarationSyntax>();
        if(options.MemberTypes.Contains(RoslynMemberType.@class))
            typeNodes = typeNodes.Concat(FilterByModifier<ClassDeclarationSyntax>(root, options));
        if(options.MemberTypes.Contains(RoslynMemberType.@record))
            typeNodes = typeNodes.Concat(FilterByModifier<RecordDeclarationSyntax>(root, options));
        if(options.MemberTypes.Contains(RoslynMemberType.@struct))
            typeNodes = typeNodes.Concat(FilterByModifier<StructDeclarationSyntax>(root, options));
        if(options.MemberTypes.Contains(RoslynMemberType.@enum))
            typeNodes = typeNodes.Concat(FilterByModifier<EnumDeclarationSyntax>(root, options));

        foreach(var node in typeNodes)
        {
            var nsNode = node
                .Ancestors()
                .OfType<BaseNamespaceDeclarationSyntax>()
                .FirstOrDefault();

            var nsName = nsNode?.Name.ToString() ?? "Global";

            string kind = node switch
            {
                ClassDeclarationSyntax => "class",
                StructDeclarationSyntax => "struct",
                RecordDeclarationSyntax => "record",
                EnumDeclarationSyntax => "enum",
                _ => "unknown"
            };

            var typeName = GetDeclarationName(node);
            var typeContext = _factory.Create(default, kind, nsName, typeName);

            ParseComments(_commentProcessor, node, typeContext);
            _collector.Add(typeContext);

            if(node is TypeDeclarationSyntax typeSyntax)
            {
                var methods = typeSyntax
                    .DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                    .Where(m =>
                    {
                        var mod = GetModifierType(m);
                        return mod.HasValue && options.MethodModifierTypes.Contains(mod.Value);
                    });

                foreach(var method in methods)
                {
                    var methodName = method.Identifier.Text;
                    var fullName = $"{typeName}.{methodName}";
                    var methodContext = _factory.Create(typeContext, "method", nsName, typeName, fullName);

                    ParseComments(_commentProcessor, method, methodContext);
                    _collector.Add(methodContext);
                }
            }
        }
    }

    private static void ParseComments(IContextInfoCommentProcessor<TContext> commentProcessor, MemberDeclarationSyntax node, TContext context)
    {
        foreach(var trivia in node.GetLeadingTrivia()
                                   .Where(t => t.IsKind(SyntaxKind.SingleLineCommentTrivia)))
        {
            var comment = trivia.ToString()
                                .TrimStart('/')
                                .Trim();
            commentProcessor.Process(comment, context);
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

        return null;
    }

    private static string GetDeclarationName(MemberDeclarationSyntax member) =>
        member switch
        {
            ClassDeclarationSyntax c => c.Identifier.Text,
            StructDeclarationSyntax s => s.Identifier.Text,
            RecordDeclarationSyntax r => r.Identifier.Text,
            EnumDeclarationSyntax e => e.Identifier.Text,
            _ => "???"
        };
}

internal record RoslynParserOptions(
    HashSet<RoslynAccessorModifierType> MethodModifierTypes,
    HashSet<RoslynAccessorModifierType> ClassModifierTypes,
    HashSet<RoslynMemberType> MemberTypes)
{
    public static RoslynParserOptions Default => new(
        new() { RoslynAccessorModifierType.@public, RoslynAccessorModifierType.@protected },
        new() { RoslynAccessorModifierType.@public, RoslynAccessorModifierType.@protected },
        new() { RoslynMemberType.@enum,
                RoslynMemberType.@class,
                RoslynMemberType.@record,
                RoslynMemberType.@struct }
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
