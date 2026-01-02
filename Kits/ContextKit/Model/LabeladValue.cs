using System;

namespace ContextKit.Model;

public interface ILabeledValue
{
    string LabeledCaption { get; }
    object LabeledData { get; }
}


public class StringColumnWrapper : ILabeledValue
{

    public string LabeledCaption { get; }

    public object LabeledData { get; }

    public StringColumnWrapper(string caption, string data)
    {
        LabeledCaption = caption;
        LabeledData = data;
    }

    public override string ToString()
    {
        return $"{LabeledData}";
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
        return HashCode.Combine(LabeledCaption, LabeledData);
    }

}

public class IntColumnWrapper : ILabeledValue
{
    public string LabeledCaption { get; }
    public object LabeledData { get; }

    public IntColumnWrapper(string caption, int data)
    {
        LabeledCaption = caption;
        LabeledData = data;
    }

    public override string ToString()
    {
        return $"{LabeledData}";
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
        return HashCode.Combine(LabeledCaption, LabeledData);

    }

}