using System.Collections.Generic;
using System.IO;

namespace HtmlKit.Builders.Core;

// pattern: Template method
public interface IHtmlCellBuilder
{
    void Cell(TextWriter sb, IHtmlTagAttributes? attributes = null, string? innerHtml = "", bool isEncodable = true);
}
