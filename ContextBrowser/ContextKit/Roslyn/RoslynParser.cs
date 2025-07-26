using ContextBrowser.ContextKit.Model;
using ContextBrowser.ContextKit.Parser;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ContextBrowser.ContextKit.Roslyn;

// context: csharp, build
internal class RoslynParser<TContext>
    where TContext : IContextWithReferences<TContext>
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

    // context: csharp, build
    public void ParseFile(string filePath, RoslynParserOptions? options = null, bool parseContexts = true, bool parseReferences = true)
    {
        options ??= RoslynParserOptions.Default;

        var code = File.ReadAllText(filePath);
        var tree = CSharpSyntaxTree.ParseText(code);
        var root = tree.GetCompilationUnitRoot();


        var compilation = CSharpCompilation.Create("Parser")
            .AddSyntaxTrees(tree)
            .AddReferences(
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location) // можно и другие
            );

        var model = compilation.GetSemanticModel(tree);

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

            var kind = node switch
            {
                ClassDeclarationSyntax => ContextInfoElementType.@class,
                StructDeclarationSyntax => ContextInfoElementType.@struct,
                RecordDeclarationSyntax => ContextInfoElementType.@record,
                EnumDeclarationSyntax => ContextInfoElementType.@enum,
                _ => ContextInfoElementType.none
            };

            TContext? typeContext = default;

            var typeName = GetDeclarationName(node);
            if(parseContexts)
            {
                typeContext = _factory.Create(default, kind, nsName, default, typeName, null);
                ParseComments(_commentProcessor, node, typeContext);
                _collector.Add(typeContext);

                if(parseReferences)
                {
                    BuildReferences(model, node, typeContext, _collector);
                }
            }

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
                    var symbol = RoslynMethodSymbolExtractor.ExtractMethodSymbol(tree, methodName);
                    var displayname = symbol?.ToDisplayString();

                    var methodContext = _factory.Create(typeContext, ContextInfoElementType.method, nsName, typeContext, methodName, displayname);
                    ParseComments(_commentProcessor, method, methodContext);
                    _collector.Add(methodContext);

                    if(parseReferences)
                    {
                        BuildReferences(model, method, methodContext, _collector);
                    }
                }
            }
        }

        foreach(var method in _collector.GetAll()
                     .Where(x => x.ElementType == ContextInfoElementType.method &&
                                 x.InvokedBy?.Count == 0 &&
                                 x.References.Count > 0))
        {
            method.MethodOwner = method;
        }
    }

    // context: references, build
    private void BuildReferences(SemanticModel model, SyntaxNode scope, TContext current, IContextCollector<TContext> collector)
    {
        if(!collector.ByFullName.TryGetValue(current.DisplayName, out var caller))
        {
            Console.WriteLine($"[MISS      ] {current.DisplayName} not found in collector.ByFullName");
            return;
        }

        foreach(var invocation in scope.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            var symbolInfo = model.GetSymbolInfo(invocation);

            if(symbolInfo.Symbol is not IMethodSymbol methodSymbol)
            {
                //Console.WriteLine($"[UNRESOLVED] {invocation.ToString()}");
                continue;
            }

            var calleeFullName = methodSymbol.ToDisplayString();
            if(!collector.ByFullName.TryGetValue(calleeFullName, out var callee))
            {
                //Console.WriteLine($"[MISS      ] {fullName} not found in collector.ByFullName");
                continue;
            }

            caller.References.Add(callee);

            callee.InvokedBy ??= new HashSet<TContext>();
            callee.InvokedBy.Add(caller);
        }
    }

    // context: csharp, build, comment
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

    // context: csharp, read, node
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

    // context: csharp, read, modifier
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

    // context: csharp, read, modifier
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

    // context: csharp, read, declaration
    private static string GetDeclarationName(MemberDeclarationSyntax member) =>
        member switch
        {
            ClassDeclarationSyntax c => c.Identifier.Text,
            StructDeclarationSyntax s => s.Identifier.Text,
            RecordDeclarationSyntax r => r.Identifier.Text,
            EnumDeclarationSyntax e => e.Identifier.Text,
            BaseNamespaceDeclarationSyntax nn => nn.Name.ToFullString(),
            _ => "???"
        };
}
