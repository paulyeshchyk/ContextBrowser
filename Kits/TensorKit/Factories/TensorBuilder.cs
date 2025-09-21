using System;
using System.Linq;
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
    /// <typeparam name="TTensor">Тип тензор-ключа.</typeparam>
    /// <param name="permutationType">Тип перестановки (порядок).</param>
    /// <param name="inputDimensions">Массив исходных измерений.</param>
    /// <param name="createKey">Функция-фабрика для создания тензора.</param>
    /// <returns>Готовый тензор-ключ.</returns>
    TTensor BuildTensor<TTensor>(TensorPermutationType permutationType, object[] inputDimensions, Func<object[], TTensor> createKey);
}

public class TensorBuilder : ITensorBuilder
{
    public TTensor BuildTensor<TTensor>(TensorPermutationType permutationType, object[] inputDimensions, Func<object[], TTensor> createKey)
    {
        var permutationOrder = TensorBuilder.GetPermutationOrder(inputDimensions, permutationType);

        var reorderedDimensions = new object[inputDimensions.Length];

        for (int i = 0; i < inputDimensions.Length; i++)
        {
            var newIndex = permutationOrder[i];
            reorderedDimensions[i] = inputDimensions[newIndex];
        }

        return createKey(reorderedDimensions);
    }

    /// <summary>
    /// TransposedPermutation<br>
    /// rank 2: new int[] { 1, 0 }<br>
    /// rank 3: new int[] { 2, 1, 0 }<br>
    /// rank 4: new int[] { 3, 2, 1, 0 }<br>
    ///
    /// StandardPermutation<br>
    /// rank 2: new int[] { 0, 1 }<br>
    /// rank 3: new int[] { 0, 1, 2 }<br>
    /// rank 4: new int[] { 0, 1, 2, 3 }<br>
    /// </summary>
    /// <param name="inputDimensions"></param>
    /// <param name="permutationType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static int[] GetPermutationOrder(object[] inputDimensions, TensorPermutationType permutationType)
    {
        var rank = inputDimensions.Length;
        return permutationType switch
        {
            TensorPermutationType.Standard => Enumerable.Range(0, rank).ToArray(),
            TensorPermutationType.Transposed => Enumerable.Range(0, rank).Reverse().ToArray(),
            _ => throw new ArgumentOutOfRangeException(nameof(permutationType), $"The provided permutation type is not supported.")
        };
    }
}