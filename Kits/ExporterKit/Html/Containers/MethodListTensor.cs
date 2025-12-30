using System;
using ContextKit.Model;
using HtmlKit.Matrix;
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
            if (this[0] is ILabeledValue l)
            {
                return l.LabeledData switch
                {
                    int li => li,
                    string ls => int.Parse(ls),
                    _ => throw new Exception("LabeladValue.Data is unknown")
                };
            }
            else if (this[0] is string s)
            {
                return int.Parse(s);
            }
            else if (this[0] is int i)
            {
                return i;
            }
            else
            {
                throw new Exception("Unknown data");
            }
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