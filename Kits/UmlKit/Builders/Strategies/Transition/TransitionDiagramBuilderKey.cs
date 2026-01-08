namespace UmlKit.Infrastructure.Options;

public record TransitionDiagramBuilderKey : IDiagramBuilderKey
{
    public string ToKeyString() => "Transition";
}