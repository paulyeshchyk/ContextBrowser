using UmlKit.Model;

namespace UmlKit.PlantUmlSpecification;

/// <summary>
/// Содержит ключевые слова по умолчанию для различных типов участников.
/// </summary>
public record UmlParticipantKeywordsSet
{
    public UmlParticipantKeyword Actor { get; init; }

    public UmlParticipantKeyword Control { get; init; }
}
