using ContextBrowserKit.Options;

namespace HtmlKit.Options;

// pattern: Configuration
public record HtmlTableOptions
{
    public SummaryPlacementType SummaryPlacement { get; set; }

    public MatrixOrientationType Orientation { get; set; }

    public HtmlTableOptions(SummaryPlacementType summaryPlacement, MatrixOrientationType orientation)
    {
        SummaryPlacement = summaryPlacement;
        Orientation = orientation;
    }
}