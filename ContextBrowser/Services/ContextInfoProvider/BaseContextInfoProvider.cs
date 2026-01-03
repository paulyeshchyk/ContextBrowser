using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowser.Services.Parsing;
using ContextKit.Model;

namespace ContextBrowser.Services.ContextInfoProvider;

// Абстрактный базовый класс для провайдеров, которым нужен список контекстов.
// context: ContextInfo, read
public abstract class BaseContextInfoProvider
{
    protected readonly IParsingOrchestrator _parsingOrchestrant;

    // Поле для кэширования результата.
    private IEnumerable<ContextInfo>? _contextsList;

    // Объект для синхронизации доступа к полю.
    private readonly object _lock = new object();

    protected BaseContextInfoProvider(IParsingOrchestrator parsingOrchestrant)
    {
        _parsingOrchestrant = parsingOrchestrant;
    }

    // context: ContextInfo, read
    protected async Task<IEnumerable<ContextInfo>> GetParsedContextsAsync(CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            // Если данные уже спарсены, возвращаем их.
            if (_contextsList != null)
            {
                return _contextsList;
            }
        }

        var contextsList = await _parsingOrchestrant.ParseAsync(cancellationToken).ConfigureAwait(false);

        lock (_lock)
        {
            _contextsList = contextsList;
        }

        return _contextsList;
    }
}
