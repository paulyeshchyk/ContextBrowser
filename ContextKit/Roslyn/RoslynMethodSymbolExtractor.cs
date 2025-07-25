using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ContextBrowser.ContextKit.Roslyn;

internal static class RoslynMethodSymbolExtractor
{
    public static CSharpCompilation GetCompilation(SyntaxTree tree)
    {
        var compilation = CSharpCompilation.Create("MyAssembly")
            .AddReferences(
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location))
            .AddSyntaxTrees(tree);
        return compilation;
    }

    public static SemanticModel GetSemanticModel(SyntaxTree tree)
    {
        var compilation = GetCompilation(tree);
        return compilation.GetSemanticModel(tree);
    }

    public static IMethodSymbol? ExtractMethodSymbol(SyntaxTree tree, string methodName)
    {
        // 4. Находим MethodDeclarationSyntax
        var methodDeclaration = tree.GetRoot().DescendantNodes()
                                    .OfType<MethodDeclarationSyntax>()
                                    .FirstOrDefault(m => m.Identifier.ValueText.Equals(methodName));

        if (methodDeclaration == null)
            return null;
        SemanticModel model = GetSemanticModel(tree);
        return model.GetDeclaredSymbol(methodDeclaration);
    }

    public static IMethodSymbol? ExtractMethodSymbol(string code, string methodName)
    {
        SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
        return ExtractMethodSymbol(tree, methodName);
    }
}