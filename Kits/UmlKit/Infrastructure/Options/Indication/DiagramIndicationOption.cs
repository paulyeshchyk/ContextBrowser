using System;
using System.Text.Json.Serialization;

namespace UmlKit.Infrastructure.Options.Indication;

public record DiagramIndicationOption
{
    public bool UseAsync { get; set; }

    [JsonConstructor]
    public DiagramIndicationOption(bool useAsync)
    {
        UseAsync = useAsync;
    }
}
