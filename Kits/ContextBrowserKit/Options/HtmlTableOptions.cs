using TensorKit.Model;

namespace ContextBrowserKit.Options;

// pattern: Configuration
// parsing: error
public record HtmlTableOptions
{
    public SummaryPlacementType SummaryPlacement { get; set; }

    public TensorPermutationType Orientation { get; set; }

    public HtmlTableOptions(SummaryPlacementType summaryPlacement, TensorPermutationType orientation)
    {
        SummaryPlacement = summaryPlacement;
        Orientation = orientation;
    }
}