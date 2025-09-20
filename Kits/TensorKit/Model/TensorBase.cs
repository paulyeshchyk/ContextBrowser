﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TensorKit.Model;

/// <summary>
/// Представляет тензор-ключ для многомерных данных.
/// Этот класс принимает массив строковых измерений и
/// сохраняет их в упорядоченном виде.
/// Свойства, представляющие измерения тензора, должены соответствовать порядку в массиве dimensions.
/// </summary>
public abstract record TensorBase<TDataType> : ITensor<TDataType>
    where TDataType : notnull
{
    // Свойство Rank остается абстрактным, так как его значение зависит от наследника.
    public abstract int Rank { get; }

    // Массив для хранения измерений.
    protected readonly TDataType[] _dimensions;

    public TDataType this[int index] => _dimensions[index];

    protected TensorBase(params TDataType[] dimensions)
    {
        if (dimensions.Length != Rank)
        {
            throw new ArgumentException($"Incorrect number of dimensions provided. Expected {Rank}, but got {dimensions.Length}.", nameof(dimensions));
        }

        _dimensions = dimensions;
    }

    // Метод для получения всех измерений.
    public IEnumerable<TDataType> Dimensions => _dimensions;

    public virtual bool Equals(TensorBase<TDataType>? other)
    {
        if (other is null || Rank != other.Rank)
        {
            return false;
        }

        return _dimensions.SequenceEqual(other._dimensions);
    }

    public override int GetHashCode()
    {
        int hash = 17;
        foreach (var element in _dimensions)
        {
            if (element != null)
            {
                hash = hash * 31 + element.GetHashCode();
            }
        }
        return hash;
    }
}