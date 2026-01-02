using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Options.Export;

namespace ContextKit.Model;

// context: ContextInfo, ContextInfoMatrix, build
public class ContextInfoFiller<TTensor> : IContextInfoFiller<TTensor>
    where TTensor : notnull
{
    private readonly IEnumerable<IWordTensorBuildStrategy<TTensor>> _strategies;
    private readonly IContextElementExtractor<ContextInfo> _contextElementExtractor;

    public int Order { get; } = int.MinValue;

    public ContextInfoFiller(IEnumerable<IWordTensorBuildStrategy<TTensor>> strategies, IContextElementExtractor<ContextInfo> contextElementExtractor)
    {

        // СОРТИРОВКА: Сортируем стратегии по приоритету (чем меньше число, тем раньше она будет обработана)
        _strategies = strategies.OrderBy(s => s.Priority).ToList();

        _contextElementExtractor = contextElementExtractor;

    }

    // context: ContextInfo, ContextInfoMatrix, build
    public void Fill(IContextInfoDataset<ContextInfo, TTensor> contextInfoData, List<ContextInfo> elements,
        ExportMatrixOptions matrixOptions)
    {
        foreach (var item in elements)
        {
            var elementGroups = _contextElementExtractor.Extract(item);

            foreach (var strategy in _strategies)
            {
                //если стратегия НЕ может обработать контексты, пропускаем
                //иначе, делаем обработку ТОЛЬКО этой страгией
                if (!strategy.CanHandle(elementGroups, matrixOptions))
                {
                    continue;
                }

                var keys = strategy.BuildTensors(elementGroups);
                foreach (var key in keys)
                    contextInfoData.Add(item, key);
                break;
            }
        }
    }
}