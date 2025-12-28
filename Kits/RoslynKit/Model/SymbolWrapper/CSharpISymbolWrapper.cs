using System;
using ContextKit.Model;

namespace RoslynKit.Model.SymbolWrapper;

public class CSharpISymbolWrapper : ISymbolInfo
{
    private string _name = string.Empty;
    private string _fullname = string.Empty;
    private string _shortName = string.Empty;
    private object? _syntax;

    public string Identifier { get; private set; } = string.Empty;

    public string Namespace { get; private set; } = string.Empty;

    public string GetName() => _name;

    public string GetFullName() => _fullname;

    public string GetShortName() => _shortName;

    public object? GetSyntax() => _syntax;

    public void SetIdentifier(string identifier)
    {
        Identifier = identifier;
    }

    public void SetNamespace(string nameSpace)
    {
        Namespace = nameSpace;
    }

    public void SetName(string name)
    {
        _name = name;
    }

    public void SetShortName(string shortName)
    {
        _shortName = shortName;
    }

    public void SetFullName(string fullName)
    {
        _fullname = fullName;
    }

    public void SetSyntax(object? syntax)
    {
        _syntax = syntax;
    }

    public S GetCoSyntax<S>()
    {
        if (_syntax is not S coSyntax)
        {
            throw new Exception($"incorrect syntax, expected {typeof(S)}");
        }
        return coSyntax;
    }

    public CSharpISymbolWrapper() { }
}
