using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace RoslynKit.AWrappers;

// context: signature, build
public abstract class SignatureBuilder
{
    public bool _includeGenerics = true;
    public bool _includeParameters = true;
    public bool _includeReturnType = true;
    public bool _includeNamespace = false;
    public bool _includeContainingType = true;
    public bool _qualifyTypes = true;

    public SignatureBuilder()
    {

    }

    public SignatureBuilder(SignatureBuilder source)
    {
        _includeContainingType = source._includeContainingType;
        _includeParameters = source._includeParameters;
        _includeReturnType = source._includeReturnType;
        _includeGenerics = source._includeGenerics;
        _qualifyTypes = source._qualifyTypes;
    }

    public SignatureBuilder IncludeGenerics(bool value)
    {
        _includeGenerics = value;
        return this;
    }

    public SignatureBuilder IncludeParameters(bool value)
    {
        _includeParameters = value;
        return this;
    }

    public SignatureBuilder IncludeReturnType(bool value)
    {
        _includeReturnType = value;
        return this;
    }

    public SignatureBuilder IncludeNamespace(bool value)
    {
        _includeNamespace = value;
        return this;
    }

    public SignatureBuilder IncludeContainingType(bool value)
    {
        _includeContainingType = value;
        return this;
    }

    public SignatureBuilder SetTypeQualification(bool value)
    {
        _qualifyTypes = value;
        return this;
    }

    // context: signature, build
    public abstract string Build(ISymbol symbol);
}
