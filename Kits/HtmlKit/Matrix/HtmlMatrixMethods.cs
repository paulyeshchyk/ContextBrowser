using System;
using System.Collections.Generic;
using System.Linq;
using ContextKit.Model;
using HtmlKit.Model.Containers;

namespace HtmlKit.Matrix;

public class HtmlMatrixMethods<TDataTensor> : IHtmlMatrix
    where TDataTensor : notnull
{
    public List<ILabeledValue> rows { get; }

    public List<ILabeledValue> cols { get; }

    public ContextInfoKeyContainerTensor<TDataTensor> OwnerTensor { get; }

    public HtmlMatrixMethods(IEnumerable<int> methodIndexies, ContextInfoKeyContainerTensor<TDataTensor> ownerTensor)
    {
        OwnerTensor = ownerTensor;
        rows = methodIndexies.Select(index => new IntColumnWrapper(caption: index.ToString(), data: index)).Cast<ILabeledValue>().ToList();

        // У одноколоночной таблицы всего одна колонка с заголовком
        cols = new List<ILabeledValue> { new TensorColumnWrapper<TDataTensor>(ownerTensor) };
    }

    // Транспонирование для одноколоночной таблицы не имеет смысла,
    // но метод всё равно должен быть реализован.
    public IHtmlMatrix Transpose()
    {
        return new HtmlMatrixDomainPerAction(cols, rows);
    }
}

public class TensorColumnWrapper<TTensor> : ILabeledValue
    where TTensor : notnull
{
    private readonly ContextInfoKeyContainerTensor<TTensor> _keyContainer;

    public string LabeledCaption => "методы";
    public object LabeledData => _keyContainer;

    public TensorColumnWrapper(ContextInfoKeyContainerTensor<TTensor> keyContainer)
    {
        _keyContainer = keyContainer ?? throw new ArgumentNullException(nameof(keyContainer));
    }

    public override string ToString()
    {
        return $"{_keyContainer}";
    }

    public override bool Equals(object? obj)
    {
        if (obj is not ILabeledValue other)
        {
            return false;
        }

        return (string.Equals(this.LabeledCaption, other.LabeledCaption, StringComparison.Ordinal)) && Equals(this.LabeledData, other.LabeledData);
    }

    public override int GetHashCode()
    {
        int hash = 17;

        hash = hash * 31 + (LabeledCaption.GetHashCode());

        hash = hash * 31 + (LabeledData.GetHashCode());

        return hash;
    }
}