namespace SemanticKit.Model;

public interface ISyntaxTreeWrapperBuilder
{
    ISyntaxTreeWrapper Build(string code, string filePath, CancellationToken cancellationToken);
}
