using ContextBrowserKit.Options;
using TensorKit.Model;

namespace HtmlKit.Options;

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