namespace ContextKit.Model.Matrix;

// context: ContextInfoMatrix, model
public interface IContextInfoMatrix : IEnumerable<KeyValuePair<ContextInfoMatrixCell, List<ContextInfo>>>
{
    // context: ContextInfoMatrix, read
    IEnumerable<string> GetActions();

    // context: ContextInfoMatrix, read
    IEnumerable<string> GetDomains();

    // context: ContextInfoMatrix, read
    List<ContextInfo> GetMethodsByAction(string action);

    // context: ContextInfoMatrix, read
    List<ContextInfo> GetMethodsByDomain(string domain);

    // context: ContextInfoMatrix, create
    void Add(ContextInfo? item, ContextInfoMatrixCell toCell);

    // context: ContextInfoMatrix, read
    bool TryGetValue(ContextInfoMatrixCell key, out List<ContextInfo> value);
}