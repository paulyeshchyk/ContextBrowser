namespace TensorKit.Model;

public interface ITensorClassifier< TData>
    where TData : notnull
{
    bool IsDimensionApplicable(TData ctx, string? dimensionName, int dimensionType);

    string GetEmptyDimensionValue(int dimensionType);
}
