using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TensorKit.Model;

public interface ITensor<TDataType>
{
    int Rank { get; }

    TDataType this[int index] { get; }

    public IEnumerable<TDataType> Dimensions { get; }
}