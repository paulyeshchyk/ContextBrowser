using System;

namespace ContextBrowserKit.Commandline.Polyfills;

// context: commandline, model
[AttributeUsage(AttributeTargets.Property)]
public class CommandLineArgumentAttribute : Attribute
{
    public string Name { get; }

    public string? Description { get; }

    public Type? ImplementationType { get; } // Тип для раскрытия деталей в справке

    public CommandLineArgumentAttribute(string name, string? description = null, Type? implementationType = null)
    {
        Name = name;
        Description = description;
        ImplementationType = implementationType;
    }
}