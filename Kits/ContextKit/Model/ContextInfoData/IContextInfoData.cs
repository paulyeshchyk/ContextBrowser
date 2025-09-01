using System.Collections.Generic;

namespace ContextKit.Model.Matrix;

// context: ContextInfoMatrix, model
public interface IContextInfoData : IEnumerable<KeyValuePair<ContextInfoDataCell, List<ContextInfo>>>
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
    void Add(ContextInfo? item, ContextInfoDataCell toCell);

    // context: ContextInfoMatrix, read
    bool TryGetValue(ContextInfoDataCell key, out List<ContextInfo> value);
}