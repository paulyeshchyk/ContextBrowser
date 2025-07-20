namespace ContextBrowser.Parser;

public interface IDimensionClassifier
{
    IEnumerable<string> GetDimensions(); // напр: "action", "domain", "coverage"

    bool IsMatch(string dimension, string value); // проверка, принадлежит ли значение измерению
}

// context: model, dimension
public class DimensionClassifier : IDimensionClassifier
{
    public readonly string[] Actions = { "create", "read", "update", "delete", "validate", "share", "build" };
    public readonly string[] Domains = { "EF", "UI", "API", "Plugin" };

    // context: dimension, read
    public IEnumerable<string> GetDimensions() => new[] { "action", "domain", "coverage", "layer" };

    // context: dimension, validate
    public bool IsMatch(string dimension, string value)
    {
        return dimension switch
        {
            "action" => Actions.Contains(value),
            "domain" => Domains.Contains(value),
            "coverage" => value == "true" || value == "false" || int.TryParse(value, out _),
            "layer" => value == "true" || value == "false" || int.TryParse(value, out _),
            _ => false
        };
    }
}