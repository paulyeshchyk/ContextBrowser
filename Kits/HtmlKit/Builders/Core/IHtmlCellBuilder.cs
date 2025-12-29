using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HtmlKit.Builders.Core;

// pattern: Template method
public interface IHtmlCellBuilder
{
    Task CellAsync(TextWriter sb, IHtmlTagAttributes? attributes = null, string? innerHtml = "", bool isEncodable = true, CancellationToken cancellationToken = default);
}
