using System;
using System.IO;

namespace UmlKit.Model;

public class UmlArrow : IUmlElement
{
    public readonly UmlArrowFlowType FlowType;
    public readonly UmlArrowDirection ArrowDirection;
    private readonly UmlArrowPosition ArrowPosition;

    public UmlArrow(UmlArrowFlowType flowType = UmlArrowFlowType.Async, UmlArrowDirection direction = UmlArrowDirection.ToRight, UmlArrowPosition arrowPosition = UmlArrowPosition.AtRight)
    {
        FlowType = flowType;
        ArrowDirection = direction;
        ArrowPosition = arrowPosition;
    }

    public void WriteTo(TextWriter writer, int alignNameMaxWidth)
    {
        var flowTypeStr = FlowType.Data();
        var flowDirection = ArrowDirection.Data();

        switch (ArrowPosition)
        {
            case UmlArrowPosition.AtRight:
                writer.Write($" {flowTypeStr}{flowDirection} ");
                break;
            case UmlArrowPosition.AtLeft:
                writer.Write($" {flowDirection}{flowTypeStr} ");
                break;
        }
    }
}

public static class UmlArrowFlowTypeExt
{
    public static string Data(this UmlArrowFlowType ft)
    {
        var data = ft switch
        {
            UmlArrowFlowType.Sync => "-",
            UmlArrowFlowType.Async => "--",
            _ => throw new NotImplementedException()
        };
        return data;
    }
}

public static class UmlArrowDirectionExt
{
    public static string Data(this UmlArrowDirection ad)
    {
        var data = ad switch
        {
            UmlArrowDirection.ToLeft => "<",
            UmlArrowDirection.ToRight => ">",
            _ => throw new NotImplementedException()
        };
        return data;
    }
}

public enum UmlArrowFlowType
{
    Sync,
    Async
}

public enum UmlArrowDirection
{
    ToLeft,
    ToRight,
    None
}

public enum UmlArrowPosition
{
    AtLeft,
    AtRight
}
