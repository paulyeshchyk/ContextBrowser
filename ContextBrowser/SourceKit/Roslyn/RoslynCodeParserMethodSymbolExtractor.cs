using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ContextBrowser.SourceKit.Roslyn;

internal static class RoslynCodeParserMethodSymbolExtractor
{
    public static IMethodSymbol? ExtractMethodSymbol(string code, string methodName, IEnumerable<string> customAssemblyPaths)
    {
        SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
        return ExtractMethodSymbol(tree, methodName, customAssemblyPaths);
    }

    private static CSharpCompilation GetCompilation(SyntaxTree tree, IEnumerable<string> customAssemblypaths)
    {
        var cSharpCompilationReferences = RoslynCodeParserAssemblyReferencesBuilder.CSharpCompilationReferences();
        var cSharpCompilationCustomReferences = RoslynCodeParserAssemblyReferencesBuilder.CSharpCompilationReferences(customAssemblypaths);
        var compilation = CSharpCompilation.Create("MyAssembly")
            .AddReferences(cSharpCompilationReferences)
            .AddReferences(cSharpCompilationCustomReferences)
            .AddSyntaxTrees(tree);
        return compilation;
    }

    private static SemanticModel GetSemanticModel(SyntaxTree tree, IEnumerable<string> customAssemblyPaths)
    {
        var compilation = GetCompilation(tree, customAssemblyPaths);
        return compilation.GetSemanticModel(tree);
    }

    private static IMethodSymbol? ExtractMethodSymbol(SyntaxTree tree, string methodName, IEnumerable<string> customAssemblyPaths)
    {
        // 4. Находим MethodDeclarationSyntax
        var methodDeclaration = tree.GetRoot().DescendantNodes()
                                    .OfType<MethodDeclarationSyntax>()
                                    .FirstOrDefault(m => m.Identifier.ValueText.Equals(methodName));

        if(methodDeclaration == null)
            return null;
        SemanticModel model = GetSemanticModel(tree, customAssemblyPaths);
        return model.GetDeclaredSymbol(methodDeclaration);
    }
}