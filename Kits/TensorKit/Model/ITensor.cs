using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TensorKit.Model;

public interface ITensor
{
    int Rank { get; }

    object this[int index] { get; }

    IEnumerable<object> Dimensions { get; }
}