using System;
using ContextKit.Model;

namespace ExporterKit.Html.Pages.CoCompiler.DomainPerAction.Coverage;

public interface ICoverageValueExtractor
{
    /// <summary>
    /// Извлекает значение из объекта ContextInfo.
    /// </summary>
    /// <param name="ctx">Объект ContextInfo.</param>
    /// <returns>Целочисленное значение.</returns>
    int GetValue(ContextInfo? ctx);
}
