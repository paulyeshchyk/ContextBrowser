using ContextKit.Model;

namespace RoslynKit.Phases.ContextInfoBuilder;

public abstract class SyntaxSymbolInfoDto : ISymbolInfo
{
    public abstract string Identifier { get; }

    public abstract string Namespace { get; }

    public abstract string GetFullName();

    public abstract string GetName();

    public abstract string GetShortName();

    public abstract string DebugInfo();

    public abstract object? GetSyntax();

    public abstract void SetSyntax(object? syntax);
}