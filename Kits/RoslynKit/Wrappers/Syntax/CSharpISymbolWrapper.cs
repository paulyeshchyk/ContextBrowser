using System;
using ContextBrowserKit.Log;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;
using SemanticKit.Model;

namespace RoslynKit.Wrappers.Syntax;

public class CSharpISymbolWrapper : ISymbolInfo
{
    private string _identifier = string.Empty;
    private string _namespace = string.Empty;
    private string _name = string.Empty;
    private string _fullname = string.Empty;
    private string _shortName = string.Empty;
    private object? _syntax = null;

    public string Identifier => _identifier;

    public string Namespace => _namespace;

    public string GetName() => _name;

    public string GetFullName() => _fullname;

    public string GetShortName() => _shortName;

    public object? GetSyntax() => _syntax;

    public void SetIdentifier(string identifier)
    {
        _identifier = identifier;
    }

    public void SetNamespace(string nameSpace)
    {
        _namespace = nameSpace;
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
