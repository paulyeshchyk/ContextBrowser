namespace SemanticKit.Model;

public interface ICompilationWrapper
{
    ISemanticModelWrapper GetSemanticModel(ISyntaxTreeWrapper wrapper);
}

