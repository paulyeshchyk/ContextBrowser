using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TensorKit.Model;

public interface ITensor<TDataType>
{
    int Rank { get; }

    TDataType this[int index] { get; }

    IEnumerable<TDataType> Dimensions { get; }
}