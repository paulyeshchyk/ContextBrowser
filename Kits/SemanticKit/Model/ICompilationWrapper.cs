namespace SemanticKit.Model;

#warning make it manager
public interface ICompilationWrapper
{
    ISemanticModelWrapper GetSemanticModel(ISyntaxTreeWrapper wrapper);
}

