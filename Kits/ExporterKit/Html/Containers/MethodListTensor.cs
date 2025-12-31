using System;
using ContextKit.Model;
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
        get
        {
            return this[0] switch
            {
                ILabeledValue l => l.LabeledData switch
                {
                    int li => li,
                    string ls => int.Parse(ls),
                    _ => throw new Exception("LabeladValue.Data is unknown")
                },
                string s => int.Parse(s),
                int i => i,
                _ => throw new Exception("Unknown data"),
            };
        }
    }

    public ContextInfoKeyContainerTensor<TDataTensor> DomainPerActionTensorContainer
    {
        get
        {
            return (ContextInfoKeyContainerTensor<TDataTensor>)this[1];
        }
    }

    public MethodListTensor(params object[] dimensions) : base(dimensions) { }
}