using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ContextBrowser.SourceKit.Roslyn;

//context: csharp, model
public record struct RoslynCompilationView(SemanticModel model, SyntaxTree tree, CompilationUnitSyntax unitSyntax)
{
    public static implicit operator (SemanticModel model, SyntaxTree tree, CompilationUnitSyntax unitSyntax)(RoslynCompilationView value)
    {
        return (value.model, value.tree, value.unitSyntax);
    }

    public static implicit operator RoslynCompilationView((SemanticModel model, SyntaxTree tree, CompilationUnitSyntax unitSyntax) value)
    {
        return new RoslynCompilationView(value.model, value.tree, value.unitSyntax);
    }
}
