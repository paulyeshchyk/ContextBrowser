namespace TensorKit.Model;

public class DomainPerActionDimensionType : ITensorDimensionType
{
    public const int Action = 0;
    public const int Domain = 1;
}

public interface IDomainPerActionTensor : ITensor
{
    string Action { get; }

    string Domain { get; }
}

public record DomainPerActionTensor : TensorBase, IDomainPerActionTensor
{
    public override int Rank => 2;

    public string Action
    {
        get => (string)this[DomainPerActionDimensionType.Action];
        init => _dimensions[DomainPerActionDimensionType.Action] = value;
    }

    public string Domain
    {
        get => (string)this[DomainPerActionDimensionType.Domain];
        init => _dimensions[DomainPerActionDimensionType.Domain] = value;
    }

    public DomainPerActionTensor(params object[] dimensions) : base(dimensions) { }

    public DomainPerActionTensor(object action, object domain) : base(action, domain) { }
}
