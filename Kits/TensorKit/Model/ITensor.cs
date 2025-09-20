namespace TensorKit.Model;

public interface ITensor<TValue>
{
    int Rank { get; }

    TValue this[int index] { get; }
}