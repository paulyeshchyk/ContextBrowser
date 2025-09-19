using System.Collections.Generic;
using ContextKit.Model;
using TensorKit.Model;

namespace HtmlKit.Document;

public interface IHtmlCellColorCalculator<TKey>
    where TKey : notnull
{
    /// <summary>
    /// Вычисляет цвет для ячейки на основе данных.
    /// </summary>
    /// <param name="cell">Ключ ячейки.</param>
    /// <param name="contextInfoList">Список связанной контекстной информации.</param>
    /// <param name="index">Индекс для быстрого поиска.</param>
    /// <returns>Строка с HEX-цветом или null.</returns>
    string? CalculateBgColor(TKey cell, IEnumerable<ContextInfo>? contextInfoList, Dictionary<string, ContextInfo>? index);
}
