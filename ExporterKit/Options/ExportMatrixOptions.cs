using ContextBrowserKit.Options;
using HtmlKit.Options;

namespace ExporterKit.Options;

public record ExportMatrixOptions
{
    public UnclassifiedPriorityType UnclassifiedPriority { get; set; }

    public bool IncludeAllStandardActions { get; set; }

    public HtmlTableOptions HtmlTable { get; set; }

    public ExportMatrixOptions(UnclassifiedPriorityType unclassifiedPriority, bool includeAllStandardActions, HtmlTableOptions htmlTable)
    {
        UnclassifiedPriority = unclassifiedPriority;
        IncludeAllStandardActions = includeAllStandardActions;
        HtmlTable = htmlTable;
    }
}
