using ContextKit.Matrix;

namespace HtmlKit.Model;

// pattern: Configuration
public class HtmlTableOptions
{
    public SummaryPlacement SummaryPlacement = SummaryPlacement.AfterFirst;
    public MatrixOrientation Orientation = MatrixOrientation.DomainRows;
}