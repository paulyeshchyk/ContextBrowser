using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HtmlKit.Builders.Core;

// pattern: Template method
public interface IHtmlTagBuilder
{
    Task StartAsync(TextWriter sb, IHtmlTagAttributes? attrs = null, CancellationToken cancellationToken = default);

    Task EndAsync(TextWriter sb, CancellationToken cancellationToken = default);
}
