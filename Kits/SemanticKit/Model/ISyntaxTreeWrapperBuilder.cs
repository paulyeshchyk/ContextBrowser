namespace SemanticKit.Model;

public interface ISyntaxTreeWrapperBuilder
{
    ISyntaxTreeWrapper BuildTree(string code, string filePath, CancellationToken cancellationToken);
}
