using ContextBrowser.ContextKit.Parser;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Model;
using RoslynKit.Parser.Code;
using RoslynKit.Parser.Semantic;

namespace RoslynKit.Parser.Phases;

// context: csharp, build, contextInfo
public class RoslynPhaseParserContextBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
    protected readonly ISemanticModelBuilder _semanticModelBuilder;
    protected readonly IContextInfoCommentProcessor<TContext> _commentProcessor;
    protected readonly IContextCollector<TContext> _collector;
    protected readonly IContextFactory<TContext> _factory;
    private readonly OnWriteLog? _onWriteLog = null;
    private readonly TypeContextInfoBulder<TContext> _typeContextInfoBuilder;
    private readonly MethodContextInfoBuilder<TContext> _methodContextInfoBuilder;
    private readonly CommentSyntaxTriviaBuilder<TContext> _triviaCommentBuilder;

    public RoslynPhaseParserContextBuilder(IContextCollector<TContext> collector, IContextFactory<TContext> factory, IContextInfoCommentProcessor<TContext> commentProcessor, ISemanticModelBuilder modelBuilder, OnWriteLog? onWriteLog) : base()
    {
        _semanticModelBuilder = modelBuilder;
        _commentProcessor = commentProcessor;
        _collector = collector;
        _factory = factory;
        _onWriteLog = onWriteLog;

        _triviaCommentBuilder = new CommentSyntaxTriviaBuilder<TContext>(_commentProcessor);

        _typeContextInfoBuilder = new TypeContextInfoBulder<TContext>(_collector, _factory, _onWriteLog);
        _methodContextInfoBuilder = new MethodContextInfoBuilder<TContext>(_collector, _factory, _onWriteLog, default);
    }

    // context: csharp, build, contextInfo
    public void ParseCode(string code, string filePath, RoslynCodeParserOptions options, CancellationToken cancellationToken)
    {
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Info, $"Parse code from file: {filePath}", LogLevelNode.Start);
        var compilationView = _semanticModelBuilder.BuildModel(code, filePath, options, cancellationToken);

        var availableSyntaxies = options.GetMemberDeclarationSyntaxies(compilationView.unitSyntax);

        var model = compilationView.model;
        foreach(var typeSyntax in availableSyntaxies)
        {
            ParseTypeSyntax(typeSyntax, options, model);
        }
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Info, string.Empty, LogLevelNode.End);
    }

    // context: csharp, build, contextInfo
    public void ParseFile(string filePath, RoslynCodeParserOptions options, CancellationToken cancellationToken)
    {
        var code = File.ReadAllText(filePath);
        ParseCode(code, filePath, options, cancellationToken);
    }

    // context: csharp, build
    public void ParseFiles(IEnumerable<string> codeFiles, RoslynCodeParserOptions options, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Info, "Parsing files", LogLevelNode.Start);

        var models = _semanticModelBuilder.BuildModels(codeFiles, options, cancellationToken);
        foreach (var (tree, model) in models)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ParseDeclarations(options, tree, model, cancellationToken);
        }
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Info, string.Empty, LogLevelNode.End);
    }

    private void ParseDeclarations(RoslynCodeParserOptions options, SyntaxTree tree, SemanticModel model, CancellationToken cancellationToken)
    {
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Info, $"Parse model {tree.FilePath}", LogLevelNode.Start);

        var root = tree.GetCompilationUnitRoot(cancellationToken);

        var availableSyntaxies = options.GetMemberDeclarationSyntaxies(root);
        ParseSyntaxies(options, tree, model, availableSyntaxies);
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Info, string.Empty, LogLevelNode.End);
    }

    private void ParseSyntaxies(RoslynCodeParserOptions options, SyntaxTree tree, SemanticModel model, IEnumerable<MemberDeclarationSyntax> availableSyntaxies)
    {
        if(!availableSyntaxies.Any())
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"Model has no members: {tree.FilePath}");
        }
        foreach(var typeSyntax in availableSyntaxies)
        {
            ParseTypeSyntax(typeSyntax, options, model);
        }
    }

    private void ParseTypeSyntax(MemberDeclarationSyntax availableSyntax, RoslynCodeParserOptions options, SemanticModel model)
    {
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Info, $"Parse type syntax");

        string nsName = availableSyntax.GetNamespaceName();
        var typeContext = _typeContextInfoBuilder.BuildContextInfoForType(availableSyntax, model, nsName);
        if(typeContext == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Syntax \"{availableSyntax}\" was not resolved in {nsName}");
            return;
        }


        _triviaCommentBuilder.ParseComments(availableSyntax, typeContext);

        ParseMethodDeclarations(availableSyntax, options, model, nsName, typeContext);
    }

    //context: csharp, build
    protected void ParseMethodDeclarations(MemberDeclarationSyntax availableSyntax, RoslynCodeParserOptions options, SemanticModel semanticModel, string nsName, TContext typeContext)
    {
        if(availableSyntax is not TypeDeclarationSyntax typeSyntax)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"[{typeContext.Name}]:Syntax is not TypeDeclaration syntax");

            return;
        }

        var methodDeclarationSyntaxies = typeSyntax.FilteredMethodsList(options);
        if(!methodDeclarationSyntaxies.Any())
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[{typeContext.Name}]:Syntax has no methods in List");
            return;
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Info, $"[{typeContext.Name}]:Iterating methods", LogLevelNode.Start);

        var buildItems = _methodContextInfoBuilder.BuildContextInfoForMethods(semanticModel, nsName, typeContext, methodDeclarationSyntaxies);
        foreach(var item in buildItems)
        {
            _triviaCommentBuilder.ParseComments(item.Item2, item.Item1);
        }
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Info, string.Empty, LogLevelNode.End);
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