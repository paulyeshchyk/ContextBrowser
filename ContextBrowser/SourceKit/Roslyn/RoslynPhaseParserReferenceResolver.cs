using ContextBrowser.ContextKit.Model;
using ContextBrowser.ContextKit.Parser;
using ContextBrowser.extensions;
using ContextBrowser.LoggerKit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ContextBrowser.SourceKit.Roslyn;

// context: csharp, read
public class RoslynPhaseParserReferenceResolver<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private ISemanticInvocationResolver _semanticInvocationResolver;
    private ISemanticModelBuilder _semanticModelBuilder;
    protected readonly IContextCollector<TContext> _collector;
    private OnWriteLog? _onWriteLog = null;

    public RoslynPhaseParserReferenceResolver(IContextCollector<TContext> collector, ISemanticModelBuilder modelBuilder, ISemanticInvocationResolver semanticInvocationResolver, OnWriteLog? onWriteLog = null) : base()
    {
        _semanticInvocationResolver = semanticInvocationResolver;
        _semanticModelBuilder = modelBuilder;
        _collector = collector;
        _onWriteLog = onWriteLog;
    }

    // context: csharp, read
    public void ParseCode(string code, string filePath, RoslynCodeParserOptions options)
    {
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"parse code: {filePath}");


        // 1. Достаём дерево из файла
        var syntaxTree = CSharpSyntaxTree.ParseText(code, path: filePath, cancellationToken: CancellationToken.None);

        // 2. Получаем сохранённую модель из хранилища
        var semanticModel = _semanticModelBuilder.ModelStorage.GetModel(syntaxTree);
        if(semanticModel == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"SemanticModel not found for {filePath}");
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
    public void ParseFile(string filePath, RoslynCodeParserOptions options)
    {
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"read file: {filePath}");

        var code = File.ReadAllText(filePath);
        ParseCode(code, filePath, options);
    }

    // context: csharp, read
    protected void BuildReferences(TContext callerContext, IContextCollector<TContext> collector)
    {
        if(string.IsNullOrWhiteSpace(callerContext.SymbolName) || !collector.ByFullName.TryGetValue(callerContext.SymbolName, out var callerContextInfo))
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"Symbol {callerContext.SymbolName} not found in collector.ByFullName");
            return;
        }
        var callerSyntaxNode = callerContext.SyntaxNode;
        if(callerSyntaxNode == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"SyntaxNode {callerContext.SymbolName} is not defined");
            return;
        }

        var invocationList = callerSyntaxNode.DescendantNodes().OfType<InvocationExpressionSyntax>();
        if(!invocationList.Any())
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"No invocation found for {callerContext.Name}");
        }

        foreach(var invocation in invocationList)
        {
            var methodSymbol = ResolveSymbol(_semanticInvocationResolver, invocation);
            if(methodSymbol == null)
            {
                _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"Symbol was not resolved for invocation {invocation}");
                continue;
            }

            var calleeSymbolName = methodSymbol.GetFullMemberName();
            collector.LinkInvocation(callerContextInfo, calleeSymbolName, _onWriteLog);
        }
    }

    // context: csharp, read
    internal ISymbol? ResolveSymbol(ISemanticInvocationResolver semanticInvocationResolver, InvocationExpressionSyntax byInvocation)
    {
        var invocationSemanticModel = FindSemanticModel(semanticInvocationResolver, byInvocation, byInvocation.SyntaxTree);
        if(invocationSemanticModel == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Semantic model was not defined for {byInvocation}");
            return null;
        }

        return GetMethodSymbol(byInvocation, invocationSemanticModel);
    }

    // context: csharp, read
    internal SemanticModel? FindSemanticModel(ISemanticInvocationResolver semanticInvocationResolver, InvocationExpressionSyntax invocation, SyntaxTree? syntaxTree)
    {
        if(syntaxTree == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Tree was not provided for invocation {invocation}");

            return null;
        }

        return semanticInvocationResolver.Resolve(syntaxTree);
    }

    // context: csharp, read
    internal IMethodSymbol? GetMethodSymbol(InvocationExpressionSyntax byInvocation, SemanticModel semanticModel)
    {
        // 4. Получаем символ
        var symbolInfo = semanticModel.GetSymbolInfo(byInvocation, CancellationToken.None);
        if(symbolInfo.Symbol == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Verb, $"No SymbolInfo was found for {byInvocation}");
            return null;
        }

        if(symbolInfo.Symbol is not IMethodSymbol result)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"SymbolInfo was found for {byInvocation}, but it has no MethodSymbol");
            return null;
        }

        return result;
    }
}

// context: csharp, builder
internal static class TContextCollectionExts
{
    // context: csharp, update
    public static void LinkInvocation<TContext>(this IContextCollector<TContext> collector, TContext callerContextInfo, string? calleeSymbolName, OnWriteLog? _onWriteLog = null)
        where TContext : IContextWithReferences<TContext>
    {
        if(string.IsNullOrWhiteSpace(calleeSymbolName))
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"[MISS] Callee symbol name is empty");
            return;
        }
        if(!collector.ByFullName.TryGetValue(calleeSymbolName, out var calleeContextInfo))
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"[MISS] \ncaller: [{callerContextInfo.SymbolName}]\nwants:  [{calleeSymbolName}]");

            return;
        }

        callerContextInfo.References.Add(calleeContextInfo);
        calleeContextInfo.InvokedBy.Add(callerContextInfo);
    }
}


// context: csharp, builder
internal static class RoslynCodeParserOptionsExts
{
    //context: csharp, read
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
        if(options.MemberTypes.Contains(RoslynCodeParserMemberType.@interface))
            typeNodes = typeNodes.Concat(FilterByModifier<InterfaceDeclarationSyntax>(root, options));

        typeNodes = typeNodes.Concat(FilterByModifier<DelegateDeclarationSyntax>(root, options));

        return typeNodes;
    }

    //context: csharp, read
    internal static RoslynCodeParserAccessorModifierType? GetClassModifierType<T>(T member)
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

    //context: csharp, read
    internal static IEnumerable<T> FilterByModifier<T>(SyntaxNode root, RoslynCodeParserOptions options)
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