using System;
using System.Text.Json.Serialization;

namespace UmlKit.Infrastructure.Options.Activation;

public record DiagramInvocationOption
{
    public bool UseInvocation { get; set; }

    public bool UseReturn { get; set; }

    [JsonConstructor]
    public DiagramInvocationOption(bool useInvocation, bool useReturn)
    {
        UseInvocation = useInvocation;
        UseReturn = useReturn;
    }
}