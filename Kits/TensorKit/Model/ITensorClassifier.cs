namespace ContextKit.Model.Classifier;

public interface ITensorClassifier<TDimensionType, TData>
    where TDimensionType : ITensorDimensionType
    where TData : class
{
    bool IsDimensionApplicable(TData ctx, string? dimensionName, int dimensionType);

    string GetEmptyDimensionValue(int dimensionType);
}
