using System.ComponentModel.DataAnnotations;
using HtmlKit.Model.Containers;
using TensorKit.Model;
using TensorKit.Model.DomainPerAction;

namespace ExporterKit.Html.Containers;

public interface IMethodListTensor : ITensor
{
    int MethodIndex { get; }

    ContextInfoKeyContainerTensor<DomainPerActionTensor> DomainPerActionTensorContainer { get; }
}

public record MethodListTensor : TensorBase, IMethodListTensor
{
    public override int Rank => 2;

    public int MethodIndex
    {
        get => (int)this[0];
        init => _dimensions[0] = value;
    }

    public ContextInfoKeyContainerTensor<DomainPerActionTensor> DomainPerActionTensorContainer
    {
        get => (ContextInfoKeyContainerTensor<DomainPerActionTensor>)this[1];
        init => _dimensions[1] = value;
    }

    public MethodListTensor(params object[] dimensions) : base(dimensions) { }
}