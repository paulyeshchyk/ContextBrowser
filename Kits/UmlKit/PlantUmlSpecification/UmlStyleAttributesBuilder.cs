namespace UmlKit.PlantUmlSpecification;

public class UmlStyleAttributesBuilder
{
    private readonly string _name;
    private readonly UmlStyleAttributes _attributes = new();

    public UmlStyleAttributesBuilder(string name)
    {
        _name = name;
    }

    public UmlStyleAttributesBuilder BackgroundColor(string color)
    {
        _attributes.Add("BackgroundColor", color);
        return this;
    }

    public UmlStyleAttributesBuilder LineColor(string color)
    {
        _attributes.Add("LineColor", color);
        return this;
    }

    public UmlStyleAttributesBuilder HyperlinkColor(string color)
    {
        _attributes.Add("HyperlinkColor", color);
        return this;
    }

    public UmlStyleAttributesBuilder HyperlinkUnderlineThickness(int thickness)
    {
        _attributes.Add("HyperlinkUnderlineThickness", thickness.ToString());
        return this;
    }

    // Вы можете добавлять другие атрибуты по мере необходимости

    public UmlStyle Build() => new UmlStyle(_name, _attributes);
}