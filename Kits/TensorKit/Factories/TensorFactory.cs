using System;

namespace TensorKit.Factories;

/// <summary>
/// Фабрика для создания многомерных тензор-ключей.
/// </summary>
/// <typeparam name="TKey">Тип тензор-ключа, который будет создан.</typeparam>
public interface ITensorFactory<TKey>
{
    /// <summary>
    /// Создает новый ключ на основе массива строковых измерений.
    /// </summary>
    /// <param name="dimensions">Массив измерений.</param>
    /// <returns>Экземпляр тензор-ключа указанного типа.</returns>
    TKey Create(string[] dimensions);
}

/// <summary>
/// Реализация фабрики для создания многомерных тензор-ключей.
/// </summary>
/// <typeparam name="TKey">Тип тензор-ключа.</typeparam>
public class TensorFactory<TKey> : ITensorFactory<TKey>
{
    private readonly Func<string[], TKey> _creationFunc;

    /// <summary>
    /// Инициализирует новый экземпляр TensorFactory.
    /// </summary>
    /// <param name="creationFunc">Функция, которая знает, как создать TKey из массива строк.</param>
    public TensorFactory(Func<string[], TKey> creationFunc)
    {
        _creationFunc = creationFunc;
    }

    /// <summary>
    /// Создает тензор-ключ, используя внутреннюю функцию создания.
    /// </summary>
    /// <param name="dimensions">Массив измерений для тензора.</param>
    /// <returns>Готовый тензор-ключ.</returns>
    public TKey Create(string[] dimensions)
    {
        return _creationFunc(dimensions);
    }
}
