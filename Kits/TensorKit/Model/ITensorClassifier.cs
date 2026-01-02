namespace TensorKit.Model;

public interface ITensorClassifier<TDimensionType, TData>
    where TDimensionType : ITensorDimensionType
    where TData : notnull
{
    bool IsDimensionApplicable(TData ctx, string? dimensionName, int dimensionType);

    string GetEmptyDimensionValue(int dimensionType);
}
