using ContextKit.Model;
using ContextKit.Model.Matrix;

namespace ContextKit.Model;

public interface IContextInfoDataset
{
    List<ContextInfo> ContextsList { get; }
    IContextInfoData ContextInfoData { get; }
    Dictionary<string, ContextInfo> ContextLookup { get; }
}

// context: ContextInfo, ContextInfoMatrix, model
public class ContextInfoDataset : IContextInfoDataset
{
    public List<ContextInfo> ContextsList { get; private set; }
    public IContextInfoData ContextInfoData { get; private set; }
    public Dictionary<string, ContextInfo> ContextLookup { get; private set; }

    public ContextInfoDataset(List<ContextInfo> contextsList, IContextInfoData matrix, Dictionary<string, ContextInfo> contextLookup)
    {
        ContextsList = contextsList;
        ContextInfoData = matrix;
        ContextLookup = contextLookup;
    }
}