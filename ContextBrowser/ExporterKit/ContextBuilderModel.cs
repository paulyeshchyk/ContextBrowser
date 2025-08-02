using ContextKit.Model;

namespace ContextBrowser.ExporterKit;

public class ContextBuilderModel
{
    public List<ContextInfo> contextsList;
    public Dictionary<ContextContainer, List<string>> matrix;
    public Dictionary<string, ContextInfo> contextLookup;

    public ContextBuilderModel(List<ContextInfo> contextsList, Dictionary<ContextContainer, List<string>> matrix, Dictionary<string, ContextInfo> contextLookup)
    {
        this.contextsList = contextsList;
        this.matrix = matrix;
        this.contextLookup = contextLookup;
    }
}