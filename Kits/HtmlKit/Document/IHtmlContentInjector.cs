using System.Threading;
using System.Threading.Tasks;

namespace HtmlKit.Document;

public interface IHtmlContentInjector<TTensor>
    where TTensor : notnull
{
    Task<string> InjectAsync(TTensor container, int cnt, CancellationToken cancellationToken);
}
