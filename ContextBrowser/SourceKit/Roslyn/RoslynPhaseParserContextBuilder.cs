using ContextBrowser.ContextKit.Model;
using ContextBrowser.ContextKit.Parser;
using ContextBrowser.extensions;
using ContextBrowser.LoggerKit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ContextBrowser.SourceKit.Roslyn;

// context: csharp, builder, contextInfo
public class RoslynPhaseParserContextBuilder<TContext> //: RoslynCodeParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    protected readonly ISemanticModelBuilder _semanticModelBuilder;
    protected readonly IContextInfoCommentProcessor<TContext> _commentProcessor;
    protected readonly IContextCollector<TContext> _collector;
    protected readonly IContextFactory<TContext> _factory;
    private readonly OnWriteLog? _onWriteLog = null;

    public RoslynPhaseParserContextBuilder(IContextCollector<TContext> collector, IContextFactory<TContext> factory, IContextInfoCommentProcessor<TContext> commentProcessor, ISemanticModelBuilder modelBuilder, OnWriteLog? onWriteLog) : base()
    {
        _semanticModelBuilder = modelBuilder;
        _commentProcessor = commentProcessor;
        _collector = collector;
        _factory = factory;
        _onWriteLog = onWriteLog;
    }

    // context: csharp, builder, contextInfo
    public void ParseCode(string code, string filePath, RoslynCodeParserOptions options)
    {
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Parse code from file: {filePath}", LogLevelNode.Start);
        var compilationView = _semanticModelBuilder.BuildModel(code, filePath, options);

        var availableSyntaxies = options.GetMemberDeclarationSyntaxies(compilationView.unitSyntax);

        var model = compilationView.model;
        foreach(var typeSyntax in availableSyntaxies)
        {
            ParseTypeDeclaration(typeSyntax, options, model);
        }
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }

    // context: csharp, builder, contextInfo
    public void ParseFile(string filePath, RoslynCodeParserOptions options)
    {
        var code = File.ReadAllText(filePath);
        ParseCode(code, filePath, options);
    }

    // context: csharp, builder
    public void ParseFiles(IEnumerable<string> codeFiles, RoslynCodeParserOptions options)
    {
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, "Phase 1", LogLevelNode.Start);

        var models = _semanticModelBuilder.BuildModels(codeFiles, options);
        foreach (var (tree, model) in models)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Parse model {tree.FilePath}", LogLevelNode.Start);

            var root = tree.GetCompilationUnitRoot(CancellationToken.None);

            var availableSyntaxies = options.GetMemberDeclarationSyntaxies(root);
            if(!availableSyntaxies.Any())
            {
                _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"Model has no members: {tree.FilePath}");
            }
            foreach(var typeSyntax in availableSyntaxies)
            {
                ParseTypeDeclaration(typeSyntax, options, model);
            }
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        }
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }

    // context: csharp, builder, contextInfo
    protected TContext? BuildContextInfoForMethod(MethodDeclarationSyntax resultSyntax, SemanticModel model, string nsName, TContext? typeContext, Func<InvocationExpressionSyntax, SemanticModel?>? symbolResolver = null)
    {
        var methodName = resultSyntax.Identifier.Text;
        var methodSymbol = model.GetDeclaredSymbol(resultSyntax);
        if(methodSymbol == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Symbol \"{methodName}\" was not resolved in {nsName}");
            return default;
        }

        var fullMemberName = methodSymbol.GetFullMemberName() ?? string.Empty;
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[{methodSymbol.Name}]:Creating method context item");

        var result = _factory.Create(typeContext, ContextInfoElementType.method, nsName, typeContext, methodName, fullMemberName, resultSyntax);

        result.MethodOwner = typeContext ?? result; //Установим MethodOwner как для одиночных методов так и для остальных

        resultSyntax.ParseSingleLineComments(_commentProcessor, result);
        _collector.Add(result);

        return result;
    }

    private void ParseTypeDeclaration(MemberDeclarationSyntax availableSyntax, RoslynCodeParserOptions options, SemanticModel model)
    {
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Parse type syntax", LogLevelNode.Start);

        string nsName = availableSyntax.GetNamespaceName();
        var typeContext = BuildContextInfoForType(availableSyntax, model, nsName, null);
        ParseMethodDeclarations(availableSyntax, options, model, nsName, typeContext, null);
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }

    //context: csharp, build
    protected void ParseMethodDeclarations(MemberDeclarationSyntax availableSyntax, RoslynCodeParserOptions options, SemanticModel semanticModel, string nsName, TContext? typeContext, Func<InvocationExpressionSyntax, SemanticModel?>? symbolResolver = null)
    {
        if(availableSyntax is not TypeDeclarationSyntax typeSyntax)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"Syntax is not TypeDeclaration syntax");

            return;
        }

        var methodDeclarationSyntaxies = typeSyntax.FilteredMethodsList(options);
        if(!methodDeclarationSyntaxies.Any())
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Syntax has no methods in List");
            return;
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"Iterating methods", LogLevelNode.Start);
        foreach(var methodDeclarationSyntax in methodDeclarationSyntaxies)
        {
            BuildContextInfoForMethod(methodDeclarationSyntax, semanticModel, nsName, typeContext, symbolResolver);
        }
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }

    protected TContext? BuildContextInfoForType(MemberDeclarationSyntax callerSyntaxNode, SemanticModel model, string nsName, Func<InvocationExpressionSyntax, SemanticModel?>? symbolResolver = null)
    {
        var kind = callerSyntaxNode.GetContextInfoElementType();
        var typeName = callerSyntaxNode.GetDeclarationName();

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[{typeName}]:Creating type context item");

        var result = _factory.Create(default, kind, nsName, default, typeName, null, callerSyntaxNode);
        callerSyntaxNode.ParseSingleLineComments(_commentProcessor, result);
        _collector.Add(result);

        return result;
    }
}

internal static class MemberDeclarationSyntaxExts
{
    private const string SFakeDeclaration = "FakeDeclaration";

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

    public static string GetDeclarationName(this MemberDeclarationSyntax member) =>
        member switch
        {
            ClassDeclarationSyntax c => c.Identifier.Text,
            StructDeclarationSyntax s => s.Identifier.Text,
            RecordDeclarationSyntax r => r.Identifier.Text,
            EnumDeclarationSyntax e => e.Identifier.Text,
            BaseNamespaceDeclarationSyntax nn => nn.Name.ToFullString(),
            _ => SFakeDeclaration
        };

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