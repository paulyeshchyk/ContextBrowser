namespace SemanticKit.Model;

public interface IInvocationParser
{
    void ParseCode(string code, string filePath, CancellationToken cancellationToken);

    void ParseFile(string filePath, CancellationToken cancellationToken);

    void ParseFiles(string[] filePaths, CancellationToken cancellationToken);
}
