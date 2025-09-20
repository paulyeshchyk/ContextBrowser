using System;
using ContextKit.Model.Classifier;

namespace TensorKit.Model.DomainPerAction;

public class DomainPerActionDimensionType : ITensorDimensionType
{
    public const int Action = 0;
    public const int Domain = 1;
}

public interface IDomainPerActionTensor<TDataType> : ITensor<TDataType>
    where TDataType : notnull
{
    TDataType Action { get; }

    TDataType Domain { get; }
}

public record DomainPerActionTensor : TensorBase<string>, IDomainPerActionTensor<string>
{
    public override int Rank => 2;

    public string Action
    {
        get => this[0];
        init => _dimensions[0] = value;
    }

    public string Domain
    {
        get => this[1];
        init => _dimensions[1] = value;
    }

    public DomainPerActionTensor(params string[] dimensions) : base(dimensions) { }

    public DomainPerActionTensor(string action, string domain) : base(action, domain) { }
}
