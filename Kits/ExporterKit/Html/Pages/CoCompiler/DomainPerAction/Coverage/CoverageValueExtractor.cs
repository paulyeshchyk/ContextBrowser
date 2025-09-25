using ContextKit.Model;

namespace ExporterKit.Html.Pages.CoCompiler.DomainPerAction.Coverage;

public class CoverageValueExtractor : ICoverageValueExtractor
{
    private const string SCoverageAttributeName = "coverage";

    /// <summary>
    /// Извлекает значение "coverage" из словаря Dimensions.
    /// </summary>
    public int GetValue(ContextInfo? ctx)
    {
        if (ctx == null)
            return 0;

        if (!ctx.Dimensions.TryGetValue(SCoverageAttributeName, out var raw))
        {
            return 0;
        }

        return int.TryParse(raw, out var v) ? v : 0;
    }
}
