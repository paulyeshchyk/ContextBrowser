namespace ContextBrowser.DiagramFactory.Model;

public enum DiagramDetailLevel
{
    Summary,    // Показываем только взаимодействия между контекстами
    Method,     // Показываем имена вызываемых методов
    Full        // Показываем "Run()", возвраты, возможно параметры
}