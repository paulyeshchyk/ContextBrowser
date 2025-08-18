using ContextKit.Model;
using ContextKit.Model.Matrix;

namespace ExporterKit.Model;

// context: ContextInfo, ContextInfoMatrix, model
public class ContextBuilderModel
{
    public List<ContextInfo> contextsList;
    public IContextInfoMatrix matrix;
    public Dictionary<string, ContextInfo> contextLookup;

    public ContextBuilderModel(List<ContextInfo> contextsList, IContextInfoMatrix matrix, Dictionary<string, ContextInfo> contextLookup)
    {
        this.contextsList = contextsList;
        this.matrix = matrix;
        this.contextLookup = contextLookup;
    }
}