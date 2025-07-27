using ContextBrowser.ContextKit.Model;
using ContextBrowser.ContextKit.Parser;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ContextBrowser.SourceKit.Roslyn;

// context: csharp, read
public class RoslynCodeParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly IContextCollector<TContext> _collector;
    private readonly IContextFactory<TContext> _factory;
    private readonly IContextInfoCommentProcessor<TContext> _commentProcessor;

    public RoslynCodeParser(IContextCollector<TContext> collector, IContextFactory<TContext> factory, IContextInfoCommentProcessor<TContext> commentProcessor)
    {
        _collector = collector;
        _factory = factory;
        _commentProcessor = commentProcessor;
    }

    // context: csharp, read
    public void ParseFile(string filePath, RoslynCodeParserOptions options, RoslynContextParserOptions contextOptions)
    {
        var code = File.ReadAllText(filePath);
        ParseCode(code, options, contextOptions);
    }

    // context: csharp, read
    public void ParseCode(string code, RoslynCodeParserOptions options, RoslynContextParserOptions contextOptions)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code, cancellationToken: CancellationToken.None);
        var root = syntaxTree.GetCompilationUnitRoot(CancellationToken.None);

        var compilation = CSharpCompilation.Create("Parser")
            .AddSyntaxTrees(syntaxTree)
            .AddReferences(RoslynCodeParserAssemblyReferencesBuilder.CSharpCompilationReferences())
            .AddReferences(RoslynCodeParserAssemblyReferencesBuilder.CSharpCompilationReferences(options.CustomAssembliesPaths));

        var model = compilation.GetSemanticModel(syntaxTree);

        var availableSyntaxies = options.GetMemberDeclarationSyntaxies(root);

        // 1. Парсим и регистрируем все классы и методы
        foreach(var availableSyntax in availableSyntaxies)
        {
            string nsName = availableSyntax.GetNamespaceName();
            var typeContext = BuildTypeContext(availableSyntax, contextOptions, model, nsName);
            BuildMethodContexts(availableSyntax, options, contextOptions, model, nsName, typeContext);
        }

        // 2. Повторно проставим References, теперь когда все методы уже зарегистрированы
        if(contextOptions.ParseReferences)
        {
            foreach(var method in _collector.GetAll().Where(m => m.ElementType == ContextInfoElementType.method))
            {
                BuildReferences(method, model, method.SyntaxNode!, _collector);
            }
        }

        // 3. Установим MethodOwner для одиночных методов
        foreach(var method in _collector.GetAll()
                     .Where(x => x.ElementType == ContextInfoElementType.method &&
                                 x.InvokedBy.Count == 0 &&
                                 x.References.Count > 0))
        {
            method.MethodOwner = method;
        }
    }

    //context: csharp, build
    protected void BuildMethodContexts(MemberDeclarationSyntax availableSyntax, RoslynCodeParserOptions options, RoslynContextParserOptions contextOptions, SemanticModel semanticModel, string nsName, TContext? typeContext)
    {
        if(availableSyntax is not TypeDeclarationSyntax typeSyntax)
            return;

        var methodDeclarationSyntaxies = typeSyntax.FilteredMethodsList(options);

        foreach(var methodDeclarationSyntax in methodDeclarationSyntaxies)
        {
            BuildMethodContext(methodDeclarationSyntax, contextOptions, semanticModel, nsName, typeContext);
        }
    }

    private TContext? BuildTypeContext(MemberDeclarationSyntax availableSyntax, RoslynContextParserOptions contextOptions, SemanticModel model, string nsName)
    {
        if(!contextOptions.ParseContexts)
            return default;

        var kind = availableSyntax.GetContextInfoElementType();
        var typeName = availableSyntax.GetDeclarationName();

        var result = _factory.Create(default, kind, nsName, default, typeName, null, availableSyntax);
        ParseSingleLineComments(availableSyntax, _commentProcessor, result);
        _collector.Add(result);

        if(contextOptions.ParseReferences)
        {
            BuildReferences(result, model, availableSyntax, _collector);
        }

        return result;
    }

    private TContext? BuildMethodContext(MethodDeclarationSyntax methodSyntax, RoslynContextParserOptions contextOptions, SemanticModel model, string nsName, TContext? typeContext)
    {
        var methodName = methodSyntax.Identifier.Text;
        var methodSymbol = model.GetDeclaredSymbol(methodSyntax);
        if(methodSymbol == null)
        {
            Console.WriteLine($"[UNRESOLVED SYMBOL] {methodName} in {nsName}");
            return default;
        }

        var fullMemberName = methodSymbol.GetFullMemberName() ?? string.Empty;

        var result = _factory.Create(typeContext, ContextInfoElementType.method, nsName, typeContext, methodName, fullMemberName, methodSyntax);

        ParseSingleLineComments(methodSyntax, _commentProcessor, result);
        _collector.Add(result);

        if(contextOptions.ParseReferences)
        {
            BuildReferences(result, model, methodSyntax, _collector);
        }

        return result;
    }

    private void BuildReferences(TContext methodContext, SemanticModel model, SyntaxNode scope, IContextCollector<TContext> collector)
    {
        if(string.IsNullOrWhiteSpace(methodContext.SymbolName) ||
           !collector.ByFullName.TryGetValue(methodContext.SymbolName, out var caller))
        {
            Console.WriteLine($"[MISS] {methodContext.SymbolName} not found in collector.ByFullName");
            return;
        }

        foreach(var invocation in scope.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            var symbolInfo = model.GetSymbolInfo(invocation);
            if(symbolInfo.Symbol is not IMethodSymbol methodSymbol)
            {
                Console.WriteLine($"[UNRESOLVED] {invocation}");
                continue;
            }

            var calleeSymbolName = methodSymbol.GetFullMemberName() ?? string.Empty;
            if(!collector.ByFullName.TryGetValue(calleeSymbolName, out var callee))
            {
                Console.WriteLine($"[MISS] Callee: {calleeSymbolName} not found");
                continue;
            }

            caller.References.Add(callee);
            callee.InvokedBy.Add(caller);

            // Устанавливаем MethodOwner
            if(callee.MethodOwner == null)
            {
                callee.MethodOwner = caller.MethodOwner ?? caller;
            }
        }
    }

    private static void ParseSingleLineComments(MemberDeclarationSyntax node, IContextInfoCommentProcessor<TContext> commentProcessor, TContext context)
    {
        foreach(var trivia in node.GetLeadingTrivia().Where(t => t.IsKind(SyntaxKind.SingleLineCommentTrivia)))
        {
            string comment = trivia.ExtractComment();
            commentProcessor.Process(comment, context);
        }
    }
}

internal static class SyntaxTriviaExt
{
    public static string ExtractComment(this SyntaxTrivia trivia)
    {
        return trivia.ToString()
                            .TrimStart('/')
                            .Trim();
    }
}

internal static class TypeDeclarationDyntaxExts
{
    public static IEnumerable<MethodDeclarationSyntax> FilteredMethodsList(this TypeDeclarationSyntax typeSyntax, RoslynCodeParserOptions options)
    {
        return typeSyntax.DescendantNodes().OfType<MethodDeclarationSyntax>()
            .Where(methodDeclarationSyntax =>
            {
                var mod = methodDeclarationSyntax.GetModifierType();
                return mod.HasValue && options.MethodModifierTypes.Contains(mod.Value);
            });
    }
}

internal static class MemberDeclarationSyntaxExts
{
    public static RoslynCodeParserAccessorModifierType? GetModifierType(this MethodDeclarationSyntax method)
    {
        if(method.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
            return RoslynCodeParserAccessorModifierType.@public;
        if(method.Modifiers.Any(m => m.IsKind(SyntaxKind.ProtectedKeyword)))
            return RoslynCodeParserAccessorModifierType.@protected;
        if(method.Modifiers.Any(m => m.IsKind(SyntaxKind.PrivateKeyword)))
            return RoslynCodeParserAccessorModifierType.@private;
        if(method.Modifiers.Any(m => m.IsKind(SyntaxKind.InternalKeyword)))
            return RoslynCodeParserAccessorModifierType.@internal;

        return null;
    }

    public static string GetNamespaceName(this MemberDeclarationSyntax availableSyntax)
    {
        var nameSpaceNodeSyntax = availableSyntax
            .Ancestors()
            .OfType<BaseNamespaceDeclarationSyntax>()
            .FirstOrDefault();

        return nameSpaceNodeSyntax?.Name.ToString() ?? "Global";
    }

    public static string GetDeclarationName(this MemberDeclarationSyntax member) =>
        member switch
        {
            ClassDeclarationSyntax c => c.Identifier.Text,
            StructDeclarationSyntax s => s.Identifier.Text,
            RecordDeclarationSyntax r => r.Identifier.Text,
            EnumDeclarationSyntax e => e.Identifier.Text,
            BaseNamespaceDeclarationSyntax nn => nn.Name.ToFullString(),
            _ => "???"
        };

    public static ContextInfoElementType GetContextInfoElementType(this MemberDeclarationSyntax? syntax)
    {
        return syntax switch
        {
            ClassDeclarationSyntax => ContextInfoElementType.@class,
            StructDeclarationSyntax => ContextInfoElementType.@struct,
            RecordDeclarationSyntax => ContextInfoElementType.@record,
            EnumDeclarationSyntax => ContextInfoElementType.@enum,
            _ => ContextInfoElementType.none
        };
    }
}

internal static class RoslynCodeParserOptionsExts
{
    public static IEnumerable<MemberDeclarationSyntax> GetMemberDeclarationSyntaxies(this RoslynCodeParserOptions options, CompilationUnitSyntax root)
    {
        IEnumerable<MemberDeclarationSyntax> typeNodes = Enumerable.Empty<MemberDeclarationSyntax>();
        if(options.MemberTypes.Contains(RoslynCodeParserMemberType.@class))
            typeNodes = typeNodes.Concat(FilterByModifier<ClassDeclarationSyntax>(root, options));
        if(options.MemberTypes.Contains(RoslynCodeParserMemberType.@record))
            typeNodes = typeNodes.Concat(FilterByModifier<RecordDeclarationSyntax>(root, options));
        if(options.MemberTypes.Contains(RoslynCodeParserMemberType.@struct))
            typeNodes = typeNodes.Concat(FilterByModifier<StructDeclarationSyntax>(root, options));
        if(options.MemberTypes.Contains(RoslynCodeParserMemberType.@enum))
            typeNodes = typeNodes.Concat(FilterByModifier<EnumDeclarationSyntax>(root, options));
        return typeNodes;
    }

    private static IEnumerable<T> FilterByModifier<T>(SyntaxNode root, RoslynCodeParserOptions options)
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

    private static RoslynCodeParserAccessorModifierType? GetClassModifierType<T>(T member)
        where T : MemberDeclarationSyntax
    {
        if(member.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
            return RoslynCodeParserAccessorModifierType.@public;
        if(member.Modifiers.Any(m => m.IsKind(SyntaxKind.ProtectedKeyword)))
            return RoslynCodeParserAccessorModifierType.@protected;
        if(member.Modifiers.Any(m => m.IsKind(SyntaxKind.PrivateKeyword)))
            return RoslynCodeParserAccessorModifierType.@private;
        if(member.Modifiers.Any(m => m.IsKind(SyntaxKind.InternalKeyword)))
            return RoslynCodeParserAccessorModifierType.@internal;

        return null;
    }
}
