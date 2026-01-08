using System;

namespace TensorKit.Model;

// context: model, matrix
public enum TensorPermutationType
{
    Standard,     // строки = действия, колонки = домены
    Transposed    // строки = домены, колонки = действия
}