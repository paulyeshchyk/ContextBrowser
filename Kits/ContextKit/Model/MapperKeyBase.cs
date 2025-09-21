using System;

namespace ContextKit.Model;

// расширяемый ключ
public abstract class MapperKeyBase : IEquatable<MapperKeyBase>
{
    private readonly string _value;

    protected MapperKeyBase(string value)
    {
        _value = value;
    }

    // Класс для общих значений.
    protected sealed class InnerKey : MapperKeyBase
    {
        public InnerKey(string value) : base(value) { }
    }

    // Статический метод для создания новых ключей
    public static MapperKeyBase Create(string value)
    {
        return new InnerKey(value);
    }

    public override string ToString() => _value;

    public override int GetHashCode() => _value.GetHashCode();

    public override bool Equals(object? obj) => Equals(obj as MapperKeyBase);

    public bool Equals(MapperKeyBase? other) => other is not null && _value == other._value;
}