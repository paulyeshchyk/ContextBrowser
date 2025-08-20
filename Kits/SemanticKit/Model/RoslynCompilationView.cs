namespace SemanticKit.Model;

//context: csharp, model
public record struct RoslynCompilationView(ISemanticModelWrapper model, ISyntaxTreeWrapper tree, object/*CompilationUnitSyntax*/ unitSyntax)
{
    public static implicit operator (ISemanticModelWrapper model, ISyntaxTreeWrapper tree, object unitSyntax)(RoslynCompilationView value)
    {
        return (value.model, value.tree, value.unitSyntax);
    }

    public static implicit operator RoslynCompilationView((ISemanticModelWrapper model, ISyntaxTreeWrapper tree, object unitSyntax) value)
    {
        return new RoslynCompilationView(value.model, value.tree, value.unitSyntax);
    }
}