namespace ContextBrowser.DiagramFactory.Model;

public enum DiagramDirection
{
    BiDirectional,
    Outgoing, // текущие методы вызывают других
    Incoming  // текущие методы вызываются другими
}