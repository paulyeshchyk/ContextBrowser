using System;

namespace ContextBrowserKit.Commandline.Polyfills;

// context: commandline, model
[AttributeUsage(AttributeTargets.Property)]
public class CommandLineArgumentAttribute : Attribute
{
    public string Name { get; }

    public string? Description { get; }

    public CommandLineArgumentAttribute(string name, string? description = null)
    {
        Name = name;
        Description = description;
    }
}