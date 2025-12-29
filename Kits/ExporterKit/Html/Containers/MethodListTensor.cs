using HtmlKit.Model.Containers;
using TensorKit.Model;

namespace ExporterKit.Html.Containers;

public interface IMethodListTensor<TDataTensor> : ITensor
    where TDataTensor : notnull
{
    int MethodIndex { get; }

    ContextInfoKeyContainerTensor<TDataTensor> DomainPerActionTensorContainer { get; }
}

public record MethodListTensor<TDataTensor> : TensorBase, IMethodListTensor<TDataTensor>
    where TDataTensor : notnull
{
    public override int Rank => 2;

    public int MethodIndex
    {
        get => (int)this[0];
        init => _dimensions[0] = value;
    }

    public ContextInfoKeyContainerTensor<TDataTensor> DomainPerActionTensorContainer
    {
        get => (ContextInfoKeyContainerTensor<TDataTensor>)this[1];
        init => _dimensions[1] = value;
    }

    public MethodListTensor(params object[] dimensions) : base(dimensions) { }
}