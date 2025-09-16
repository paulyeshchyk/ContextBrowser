using System;
using TensorKit.Model;

namespace TensorKit.Factories;

/// <summary>
/// Универсальный строитель, который нормализует порядок измерений
/// для многомерного тензора.
/// </summary>
public interface ITensorBuilder
{
    /// <summary>
    /// Строит тензор-ключ, нормализуя порядок его измерений
    /// в зависимости от типа перестановки.
    /// </summary>
    /// <typeparam name="TKey">Тип тензор-ключа.</typeparam>
    /// <param name="permutationType">Тип перестановки (порядок).</param>
    /// <param name="inputDimensions">Массив исходных измерений.</param>
    /// <param name="createKey">Функция-фабрика для создания тензора.</param>
    /// <returns>Готовый тензор-ключ.</returns>
    TKey BuildTensor<TKey>(TensorPermutationType permutationType, string[] inputDimensions, Func<string[], TKey> createKey);
}

public class TensorBuilder : ITensorBuilder
{
    public TKey BuildTensor<TKey>(TensorPermutationType permutationType, string[] inputDimensions, Func<string[], TKey> createKey)
    {
        if (permutationType == TensorPermutationType.Transposed)
        {
            // Логика перестановки для двумерных тензоров.
            if (inputDimensions.Length == 2)
            {
                var reorderedDimensions = new string[2];
                reorderedDimensions[0] = inputDimensions[1];
                reorderedDimensions[1] = inputDimensions[0];
                return createKey(reorderedDimensions);
            }
        }

        return createKey(inputDimensions);
    }
}