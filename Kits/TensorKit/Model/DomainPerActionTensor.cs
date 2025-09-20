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
        get => this[DomainPerActionDimensionType.Action];
        init => _dimensions[DomainPerActionDimensionType.Action] = value;
    }

    public string Domain
    {
        get => this[DomainPerActionDimensionType.Domain];
        init => _dimensions[DomainPerActionDimensionType.Domain] = value;
    }

    public DomainPerActionTensor(params string[] dimensions) : base(dimensions) { }

    public DomainPerActionTensor(string action, string domain) : base(action, domain) { }
}
