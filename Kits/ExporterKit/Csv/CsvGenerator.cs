using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ContextBrowserKit.Options;
using ContextKit.Model;
using TensorKit.Factories;
using TensorKit.Model;

namespace ExporterKit.Csv;

// context: heatmap, build
// pattern: Builder
public class CsvGenerator : ICsvGenerator
{
    private readonly ITensorFactory<DomainPerActionTensor> _keyFactory;
    private readonly ITensorBuilder _keyBuilder;

    public CsvGenerator(ITensorFactory<DomainPerActionTensor> keyFactory, ITensorBuilder keyBuilder)
    {
        _keyFactory = keyFactory;
        _keyBuilder = keyBuilder;
    }

    //context: build, csv, heatmap
    public void GenerateHeatmap(IDomainPerActionContextTensorClassifier contextClassifier, Dictionary<DomainPerActionTensor, List<string>> matrix, string outputPath, UnclassifiedPriorityType unclassifiedPriority = UnclassifiedPriorityType.None)
    {
        var lines = new List<string>();

        // Отфильтруем обычные ключи
        var filteredKeys = matrix.Keys
            .Where(k => k.Action != contextClassifier.EmptyAction && k.Domain != contextClassifier.EmptyDomain)
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

        var unclassified = _keyBuilder.BuildTensor(TensorPermutationType.Standard, new[] { contextClassifier.EmptyAction, contextClassifier.EmptyDomain }, _keyFactory.Create);

        // Добавим строку для нераспознанных, если нужно
        if (includeUnclassified && matrix.ContainsKey(unclassified))
        {
            var unclassifiedCount = matrix[unclassified].Count;
            var rows = new List<string> { contextClassifier.EmptyAction };

            // Заполняем пустыми значениями для всех доменов
            foreach (var _ in domains)
                rows.Add("0");

            // Добавим колонку "NoDomain" в конец, если она не была в списке
            rows.Add(unclassifiedCount.ToString());

            // Обновим заголовок, добавив "NoDomain" в конец
            if (!domains.Contains(contextClassifier.EmptyDomain))
                lines[0] += $";{contextClassifier.EmptyDomain}";

            lines.Add(string.Join(";", rows));
        }

        File.WriteAllLines(outputPath, lines);
    }
}