using System;
using System.Threading;
using System.Threading.Tasks;

namespace ContextKit.Model;

public interface IContextInfoMapperProvider
{
    Task<IContextKeyMap<ContextInfo>> GetMapperAsync(MapperKeyBase mapperType, CancellationToken cancellationToken);
}

// расширяемый ключ

public abstract class MapperKeyBase : IEquatable<MapperKeyBase>
{
    private readonly string _value;

    protected MapperKeyBase(string value)
    {
        _value = value;
    }

    public static readonly MapperKeyBase CommonValue = new CommonValueType("CommonValue");

    public override string ToString() => _value;

    public override int GetHashCode() => _value.GetHashCode();

    public override bool Equals(object obj) => Equals(obj as MapperKeyBase);

    public bool Equals(MapperKeyBase other) => !(other is null) && _value == other._value;

    // Класс для общих значений
    private sealed class CommonValueType : MapperKeyBase
    {
        public CommonValueType(string value) : base(value) { }
    }
}