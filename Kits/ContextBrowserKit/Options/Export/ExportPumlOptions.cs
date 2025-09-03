namespace ContextBrowserKit.Options.Export;

public record ExportPumlOptions
{
    public PumlInjectionType InjectionType { get; }

    public ExportPumlOptions(PumlInjectionType injectionType)
    {
        InjectionType = injectionType;
    }
}

public enum PumlInjectionType
{
    include,
    inject
}