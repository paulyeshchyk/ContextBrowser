using System;

namespace TensorKit.Model;

/// <summary>
/// Представляет тензор-ключ для многомерных данных.
/// Этот класс принимает массив строковых измерений и
/// сохраняет их в упорядоченном виде.
/// Свойства, представляющие измерения тензора, должены соответствовать порядку в массиве dimensions.
/// </summary>
/// 
public record DomainPerActionTensor : ITensor<string>
{
    public int Rank => 2;

    //rank 2: row
    public string Action { get; init; }

    //rank 2: col
    public string Domain { get; init; }

    public string this[int index] => GetDimensions()[index];

    public string[] GetDimensions() => new string[] { Action, Domain };

    /// <summary>
    /// Конструктор, который принимает массив строковых измерений.
    /// Он выполняет валидацию и присваивает значения свойствам.
    /// </summary>
    /// <param name="dimensions">Массив измерений, представляющий тензор.</param>
    public DomainPerActionTensor(params string[] dimensions)
    {
        // Внутренняя валидация: проверяем, что количество измерений
        // соответствует ожидаемому (в данном случае, 2).
        if (dimensions.Length != Rank)
        {
            throw new ArgumentException($"Incorrect number of dimensions provided. Expected {Rank}.", nameof(dimensions));
        }

        // Порядок здесь имеет критическое значение.
        Action = dimensions[0];
        Domain = dimensions[1];
    }
}
