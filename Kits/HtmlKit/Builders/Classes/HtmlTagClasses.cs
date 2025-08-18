namespace HtmlKit.Classes;

internal static class HtmlTagClasses
{
    public static class Row
    {
        public const string Meta = "row_meta";
        public const string Summary = "row_summary";
        public const string Data = "row_data";
    }

    public static class Cell
    {
        public const string SummaryCaption = "cell_summary_caption";
        public const string TotalSummary = "cell_total_summary";
        public const string ColSummary = "cell_colsummary";
        public const string RowSummary = "cell_rowsummary";
        public const string ColMeta = "cell_col_meta";
        public const string RowMeta = "cell_row_meta";
        public const string Data = "cell_data";
        public const string ActionDomain = "cell_actiondomain";
    }

    public static class Page
    {
        public static string Html = string.Empty;
        public static string Head = string.Empty;
        public static string Body = string.Empty;
        public static string Title = string.Empty;
        public static string Table = "data-table";
    }
}