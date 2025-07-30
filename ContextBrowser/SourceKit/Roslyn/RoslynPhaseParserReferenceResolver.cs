using ContextBrowser.ContextKit.Model;
using ContextBrowser.ContextKit.Parser;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ContextBrowser.SourceKit.Roslyn;

// context: csharp, read
public class RoslynPhaseParserReferenceResolver<TContext> //: RoslynCodeParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private ISemanticInvocationResolver _semanticInvocationResolver;
    private ISemanticModelBuilder _semanticModelBuilder;
    protected readonly IContextCollector<TContext> _collector;

    public RoslynPhaseParserReferenceResolver(IContextCollector<TContext> collector, ISemanticModelBuilder modelBuilder, ISemanticInvocationResolver semanticInvocationResolver) : base()
    {
        _semanticInvocationResolver = semanticInvocationResolver;
        _semanticModelBuilder = modelBuilder;
        _collector = collector;
    }

    // context: csharp, read
    public void ParseCode(string code, string filePath, RoslynCodeParserOptions options, RoslynContextParserOptions contextOptions)
    {
        // 1. Достаём дерево из файла
        var syntaxTree = CSharpSyntaxTree.ParseText(code, path: filePath, cancellationToken: CancellationToken.None);

        // 2. Получаем сохранённую модель из хранилища
        var semanticModel = _semanticModelBuilder.ModelStorage.GetModel(syntaxTree);
        if(semanticModel == null)
        {
            Console.WriteLine($"[MISS] SemanticModel not found for {filePath}");
            return;
        }

        // 3. Получаем рут узел
        var root = syntaxTree.GetCompilationUnitRoot(CancellationToken.None);

        // 4. Обрабатываем все методы, зарегистрированные в коллекторе
        foreach(var method in _collector.GetAll().Where(m => m.ElementType == ContextInfoElementType.method))
        {
            BuildReferences(method, _collector);
        }
    }

    // context: csharp, read
    public void ParseFile(string filePath, RoslynCodeParserOptions options, RoslynContextParserOptions contextOptions)
    {
        var code = File.ReadAllText(filePath);
        ParseCode(code, filePath, options, contextOptions);
    }

    // context: csharp, read
    protected void BuildReferences(TContext callerContext, IContextCollector<TContext> collector)
    {
        if(string.IsNullOrWhiteSpace(callerContext.SymbolName) || !collector.ByFullName.TryGetValue(callerContext.SymbolName, out var callerContextInfo))
        {
            Console.WriteLine($"[MISS] {callerContext.SymbolName} not found in collector.ByFullName");
            return;
        }
        var callerSyntaxNode = callerContext.SyntaxNode;
        if(callerSyntaxNode == null)
        {
            Console.WriteLine($"[MISS] {callerContext.SymbolName} SyntaxNode is not defined");
            return;
        }

        foreach(var invocation in callerSyntaxNode.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            var methodSymbol = ResolveSymbol(_semanticInvocationResolver, invocation);
            if(methodSymbol == null)
            {
                continue;
            }

            var calleeSymbolName = methodSymbol.GetFullMemberName();
            collector.LinkInvocation(callerContextInfo, calleeSymbolName);
        }
    }

    internal static ISymbol? ResolveSymbol(ISemanticInvocationResolver semanticInvocationResolver, InvocationExpressionSyntax byInvocation)
    {
        var invocationSemanticModel = FindSemanticModel(semanticInvocationResolver, byInvocation, byInvocation.SyntaxTree);
        if(invocationSemanticModel == null)
        {
            Console.WriteLine($"[ERROR] semantic model was not defined for {byInvocation}");
            return null;
        }

        return GetMethodSymbol(byInvocation, invocationSemanticModel);
    }

    private static SemanticModel? FindSemanticModel(ISemanticInvocationResolver semanticInvocationResolver, InvocationExpressionSyntax invocation, SyntaxTree? syntaxTree)
    {
        if(syntaxTree == null)
        {
            Console.WriteLine($"[ERROR] tree was not provided for invocation {invocation}");

            return null;
        }

        return semanticInvocationResolver.Resolve(syntaxTree);
    }

    private static IMethodSymbol? GetMethodSymbol(InvocationExpressionSyntax byInvocation, SemanticModel semanticModel)
    {
        // 4. Получаем символ
        var symbolInfo = semanticModel.GetSymbolInfo(byInvocation, CancellationToken.None);
        if(symbolInfo.Symbol == null)
        {
            Console.WriteLine($"[ERROR] no SymbolInfo was found for {byInvocation}");
            return null;
        }

        if(symbolInfo.Symbol is not IMethodSymbol result)
        {
            Console.WriteLine($"[MISS] SymbolInfo was found for {byInvocation}, but it has no MethodSymbol");
            return null;
        }

        return result;
    }
}

internal static class TContextCollectionExts
{
    public static void LinkInvocation<TContext>(this IContextCollector<TContext> collector, TContext callerContextInfo, string? calleeSymbolName)
        where TContext : IContextWithReferences<TContext>
    {
        if(string.IsNullOrWhiteSpace(calleeSymbolName))
        {
            Console.WriteLine($"[MISS] Callee symbol name is empty not found");
            return;
        }
        if(!collector.ByFullName.TryGetValue(calleeSymbolName, out var calleeContextInfo))
        {
            Console.WriteLine($"[MISS] Callee: {calleeSymbolName} not found");
            return;
        }

        callerContextInfo.References.Add(calleeContextInfo);
        calleeContextInfo.InvokedBy.Add(callerContextInfo);
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

    private static IEnumerable<T> FilterByModifier<T>(SyntaxNode root, RoslynCodeParserOptions options)
        where T : MemberDeclarationSyntax
    {
        return root.DescendantNodes()
                   .OfType<T>()
                   .Where(node =>
                   {
                       var modifier = GetClassModifierType(node);
                       return modifier.HasValue && options.ClassModifierTypes.Contains(modifier.Value);
                   });
    }
}