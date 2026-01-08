using System;
namespace UmlKit.Infrastructure.Options.Indication;

public record DiagramIndicationOption
{
    public bool UseAsync { get; set; }

    public DiagramIndicationOption(bool useAsync)
    {
        UseAsync = useAsync;
    }
}
