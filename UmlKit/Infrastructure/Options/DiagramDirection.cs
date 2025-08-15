namespace UmlKit.Infrastructure.Options;

// parsing: error
public enum DiagramDirection
{
    BiDirectional,
    Outgoing, // текущие методы вызывают других
    Incoming  // текущие методы вызываются другими
}