using ContextKit.Model;
using ContextKit.Model.Matrix;

namespace ExporterKit.Model;

// context: ContextInfo, ContextInfoMatrix, model
public class ContextInfoMatrixModel
{
    public List<ContextInfo> ContextsList;
    public IContextInfoMatrix Matrix;
    public Dictionary<string, ContextInfo> ContextLookup;

    public ContextInfoMatrixModel(List<ContextInfo> contextsList, IContextInfoMatrix matrix, Dictionary<string, ContextInfo> contextLookup)
    {
        ContextsList = contextsList;
        Matrix = matrix;
        ContextLookup = contextLookup;
    }
}