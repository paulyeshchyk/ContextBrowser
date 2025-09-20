using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Classifier;
using TensorKit.Factories;
using TensorKit.Model;
using TensorKit.Model.DomainPerAction;

namespace ExporterKit.Csv;

// context: heatmap, build
// pattern: Builder
public class CsvGenerator : ICsvGenerator
{
    private readonly ITensorFactory<DomainPerActionTensor> _keyFactory;
    private readonly ITensorBuilder _keyBuilder;
    private readonly IWordRoleClassifier _wordRoleClassifier;
    private readonly IEmptyDimensionClassifier _emptyDimensionClassifier;

    public CsvGenerator(ITensorFactory<DomainPerActionTensor> keyFactory, ITensorBuilder keyBuilder, IAppOptionsStore appOptionsStore)
    {
        _keyFactory = keyFactory;
        _keyBuilder = keyBuilder;
        _wordRoleClassifier = appOptionsStore.GetOptions<IWordRoleClassifier>();
        _emptyDimensionClassifier = appOptionsStore.GetOptions<IEmptyDimensionClassifier>();
    }

    //context: build, csv, heatmap
    public void GenerateHeatmap(ITensorClassifierDomainPerActionContext contextClassifier, Dictionary<DomainPerActionTensor, List<string>> matrix, string outputPath, UnclassifiedPriorityType unclassifiedPriority = UnclassifiedPriorityType.None)
    {
        var lines = new List<string>();

        // Отфильтруем обычные ключи
        var filteredKeys = matrix.Keys
            .Where(k => k.Action != _emptyDimensionClassifier.EmptyAction && k.Domain != _emptyDimensionClassifier.EmptyDomain)
            .ToList();

        var actions = filteredKeys.Select(k => k.Action).Distinct().OrderBy(x => x).ToList();
        var domains = filteredKeys.Select(k => k.Domain).Distinct().OrderBy(x => x).ToList();

        // Заголовок
        lines.Add("Action;" + string.Join(";", domains));

        // Основная матрица
        foreach (var action in actions)
        {
            var rows = new List<string> { action };
            foreach (var domain in domains)
            {
                var contextKey = _keyBuilder.BuildTensor(TensorPermutationType.Standard, new[] { action, domain }, _keyFactory.Create);
                var count = matrix.TryGetValue(contextKey, out var methods) ? methods.Count : 0;
                rows.Add(count.ToString());
            }
            lines.Add(string.Join(";", rows));
        }

        var includeUnclassified = unclassifiedPriority != UnclassifiedPriorityType.None;

        var unclassified = _keyBuilder.BuildTensor(TensorPermutationType.Standard, new[] { _emptyDimensionClassifier.EmptyAction, _emptyDimensionClassifier.EmptyDomain }, _keyFactory.Create);

        // Добавим строку для нераспознанных, если нужно
        if (includeUnclassified && matrix.ContainsKey(unclassified))
        {
            var unclassifiedCount = matrix[unclassified].Count;
            var rows = new List<string> { _emptyDimensionClassifier.EmptyAction };

            // Заполняем пустыми значениями для всех доменов
            foreach (var _ in domains)
                rows.Add("0");

            // Добавим колонку "NoDomain" в конец, если она не была в списке
            rows.Add(unclassifiedCount.ToString());

            // Обновим заголовок, добавив "NoDomain" в конец
            if (!domains.Contains(_emptyDimensionClassifier.EmptyDomain))
                lines[0] += $";{_emptyDimensionClassifier.EmptyDomain}";

            lines.Add(string.Join(";", rows));
        }

        File.WriteAllLines(outputPath, lines);
    }
}