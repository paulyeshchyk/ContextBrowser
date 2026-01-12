using System;
using System.Collections.Generic;
using System.Linq;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.Builders;

public static class PumlBuilderSquaredLayout
{
    public static IEnumerable<IUmlElement> Build(IEnumerable<string> allElements, int? columns = null, int? rows = null)
    {
        var elements = allElements.ToList();
        int elementCount = elements.Count;

        // 1. Определение размеров сетки
        if (!columns.HasValue && !rows.HasValue)
        {
            // Если размеры не заданы, вычисляем "квадратный корень"
            columns = (int)Math.Ceiling(Math.Sqrt(elementCount));
            rows = (int)Math.Ceiling((double)elementCount / columns.Value);
        }
        else if (columns.HasValue && !rows.HasValue)
        {
            // Задана только ширина, вычисляем высоту
            rows = (int)Math.Ceiling((double)elementCount / columns.Value);
        }
        else if (!columns.HasValue && rows.HasValue)
        {
            // Задана только высота, вычисляем ширину
            columns = (int)Math.Ceiling((double)elementCount / rows.Value);
        }

        // 2. Построение сетки с помощью горизонтальных связей
        for (int i = 0; i < elementCount; i++)
        {
            // Строим горизонтальные связи
            if ((i + 1) % columns != 0 && i + 1 < elementCount)
            {
                var currentElement = elements[i];
                var nextElement = elements[i + 1];

                // Используем невидимые связи (hidden)
                yield return new UmlComponentRelation(currentElement, nextElement, UmlArrowDirection.None, "hidden");
            }
        }
    }
}
