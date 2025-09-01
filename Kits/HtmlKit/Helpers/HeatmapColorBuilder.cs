using System;

namespace HtmlKit.Helpers;

// context: color, build
internal static class HeatmapColorBuilder
{
    // context: build, color
    public static string? ToHeatmapColor(double avgCoverage)
    {
        if (avgCoverage == 0)
        {
            return null;
        }

        // Нормируем в диапазон [0,100]
        var pct = Math.Min(100, Math.Max(0, avgCoverage));

        // Вычисляем канал R и B
        int channel = (int)(255 * (100 - pct) / 100);

        // G = 255
        return $"#{channel:X2}FF{channel:X2}";
    }
}