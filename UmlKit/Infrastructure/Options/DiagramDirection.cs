namespace UmlKit.Infrastructure.Options;

public enum DiagramDirection
{
    BiDirectional,
    Outgoing, // текущие методы вызывают других
    Incoming  // текущие методы вызываются другими
}