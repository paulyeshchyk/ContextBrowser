using System.Collections.Generic;
using ContextKit.Model;
using TensorKit.Model;

namespace HtmlKit.Document;

public interface IHtmlCellStyleBuilder
{
    /// <summary>
    /// Создает строку стиля на основе данных о ячейке.
    /// </summary>
    /// <param name="cell">Ключ ячейки.</param>
    /// <param name="stInfo">Список информации о контексте.</param>
    /// <param name="index">Индекс для быстрого поиска.</param>
    /// <returns>Строка стиля HTML или null.</returns>
    string? BuildCellStyle(DomainPerActionTensor cell, IEnumerable<ContextInfo>? stInfo, Dictionary<string, ContextInfo>? index);
}
