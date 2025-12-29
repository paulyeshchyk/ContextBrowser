using Microsoft.CodeAnalysis;

namespace RoslynKit.Signature.SignatureBuilder;

// context: signature, build
public abstract class SignatureBuilder
{
    public bool IncludeGenerics;
    public bool IncludeParameters;
    public bool IncludeReturnType;
    public bool IncludeNamespace;
    public bool IncludeContainingType;
    public bool QualifyTypes;

    protected SignatureBuilder()
    {
    }

    protected SignatureBuilder(SignatureBuilder source)
    {
        IncludeContainingType = source.IncludeContainingType;
        IncludeParameters = source.IncludeParameters;
        IncludeReturnType = source.IncludeReturnType;
        IncludeGenerics = source.IncludeGenerics;
        QualifyTypes = source.QualifyTypes;
    }

    public SignatureBuilder SetIncludeGenerics(bool value)
    {
        IncludeGenerics = value;
        return this;
    }

    public SignatureBuilder SetIncludeParameters(bool value)
    {
        IncludeParameters = value;
        return this;
    }

    public SignatureBuilder SetIncludeReturnType(bool value)
    {
        IncludeReturnType = value;
        return this;
    }

    public SignatureBuilder SetIncludeNamespace(bool value)
    {
        IncludeNamespace = value;
        return this;
    }

    public SignatureBuilder SetIncludeContainingType(bool value)
    {
        IncludeContainingType = value;
        return this;
    }

    public SignatureBuilder SetTypeQualification(bool value)
    {
        QualifyTypes = value;
        return this;
    }

    // context: signature, build
    public abstract string Build(ISymbol symbol);
}
